using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using Uranium.CodeAnalysis.Binding.NodeKinds;
using Uranium.CodeAnalysis.Binding.Statements;
using Uranium.CodeAnalysis.Binding;
using Uranium.CodeAnalysis.Syntax.EvaluatorSupport;

namespace Uranium.CodeAnalysis.Lowering
{
    internal sealed class Lowerer : BoundTreeRewriter
    {
        private int _labelCount = 0;
        private Lowerer()
        { }

        private BoundLabel GenerateLabel(string name)
            => new($"{name} {++_labelCount}");
        
        public static BoundBlockStatement Lower(BoundStatement statement)
        {
            var lowerer = new Lowerer();
            var result = lowerer.RewriteStatement(statement);
            return Flatten(result);
        }

        private static BoundBlockStatement Flatten(BoundStatement statement)
        {
            var builder = ImmutableArray.CreateBuilder<BoundStatement>();
            var stack = new Stack<BoundStatement>();
            stack.Push(statement);

            while(stack.Count > 0)
            {
                var current = stack.Pop();
                if(current is BoundBlockStatement block)
                {
                    var reversedStatements = block.Statements.Reverse().ToArray();
                    for(int i = 0; i < reversedStatements.Length; i++)
                    {
                        stack.Push(reversedStatements[i]);
                    }
                }
                else
                {
                    builder.Add(current);
                }
            }

            return new(builder.ToImmutable());
        }

        protected override BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            if (node.ElseStatement is null)
            {
                // if(condition)
                // {
                // }
                // =======================================
                // if <condition>
                // gotoIfFalse <condition> end
                //     stuffs
                // end:

                var endLabel = GenerateLabel("End Label");
                var gotoFalse = new BoundConditionalGotoStatement(endLabel, node.Condition, true);
                var endLabelStatement = new BoundLabelStatement(endLabel);
                var result = new BoundBlockStatement
                    (
                        ImmutableArray.Create<BoundStatement>
                        (
                            gotoFalse,
                            node.Statement,
                            endLabelStatement
                        )
                    );
                return RewriteStatement(result);
            }
            else
            {
                // if(condition)
                // {
                // }
                // else
                // {
                // }
                // =======================================
                // if<condition>
                // gotoIfFalse <condition> else
                //     stuffs
                // else:
                //     Else stuffs
                // end:
                var elseLabel = GenerateLabel("Else Label");
                var endLabel = GenerateLabel("End Label");

                var gotoFalse = new BoundConditionalGotoStatement(elseLabel, node.Condition, true);
                var gotoEndStatement = new BoundGotoStatement(endLabel);

                var elseLabelStatement = new BoundLabelStatement(elseLabel);
                var endLabelStatement = new BoundLabelStatement(endLabel);
                var result = new BoundBlockStatement
                    (
                        ImmutableArray.Create<BoundStatement>
                        (
                            gotoFalse,
                            node.Statement,
                            gotoEndStatement,
                            elseLabelStatement,
                            node.ElseStatement,
                            endLabelStatement
                        )
                    );
                return RewriteStatement(result);

            }
        }
        protected override BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            //while <condition>
            //{
            //}
            // =======================================
            //
            // continue: 
            //     <body>
            //
            // check:
            //     gotoTrue <condition> continue     
            //
            //end:

            var checkLabel = GenerateLabel("Check Label");
            var continueLabel = GenerateLabel("Continue Label");
            var endLabel = GenerateLabel("End Label");
            var gotoCheck = new BoundGotoStatement(checkLabel);
            var continueLabelStatement = new BoundLabelStatement(continueLabel);
            var checkLabelStatement = new BoundLabelStatement(checkLabel);
            var endLabelStatement = new BoundLabelStatement(endLabel);

            var gotoTrue = new BoundConditionalGotoStatement(continueLabel, node.Condition, false);

            if(node.Condition is BoundLiteralExpression expression && expression.Value is bool b && b)
            {
                var gotoContinue = new BoundGotoStatement(continueLabel);
                var trueResult = new BoundBlockStatement
                (
                    ImmutableArray.Create
                    (
                        continueLabelStatement,
                        node.Body,
                        checkLabelStatement,
                        gotoContinue,
                        endLabelStatement
                    )
                );
                return RewriteStatement(trueResult);
            }


            var result = new BoundBlockStatement
                (
                    ImmutableArray.Create
                    (
                        gotoCheck,
                        continueLabelStatement,
                        node.Body,
                        checkLabelStatement,
                        gotoTrue,
                        endLabelStatement
                    )
                );
            return RewriteStatement(result);

        }
        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {
            //for(var <identifier>; <identifier> <conditional operator> <condition>; <incrementation>)
            //
            //{
            //      <variable declaration>
            //      while(condition)
            //      {
            //
            //          <body>
            //          <incrementation>
            //      }
            //}
            //
            BoundVariableDeclaration? variableDeclaration = null;
            BoundBinaryExpression? condition = null;
            var trueExpression = new BoundLiteralExpression(true);
            BoundAssignmentExpression? incrementation = null;
            var body = RewriteStatement(node.Body);

            var whileLoop = new BoundWhileStatement(trueExpression, body);
            
            if(node.VariableDeclaration is not null)
            {
                variableDeclaration = (BoundVariableDeclaration)RewriteStatement(node.VariableDeclaration);
            }
            if(node.Condition is not null)
            {
                condition = (BoundBinaryExpression)RewriteExpression(node.Condition);
            }
            if(node.Increment is not null)
            {
                incrementation = (BoundAssignmentExpression)RewriteExpression(node.Increment);
            }
            
            if(variableDeclaration is null &&
               condition is null &&
               incrementation is null)
            {
                return whileLoop;
            }

            var builder = ImmutableArray.CreateBuilder<BoundStatement>();
            
            if(variableDeclaration is not null) builder.Add(variableDeclaration);

            if(incrementation is not null)
            {
                var incrementAsStatement = new BoundExpressionStatement(incrementation);
                body = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(node.Body, incrementAsStatement));
            }

            builder.Add(condition is null ? whileLoop : new BoundWhileStatement(condition, body));

            var result = new BoundBlockStatement(builder.ToImmutable());

            return RewriteStatement(result);
        }

        protected override BoundExpression RewriteBinaryExpression(BoundBinaryExpression node)
        {
            if(node.Left is BoundBinaryExpression b)
            {
                node.Left = RewriteBinaryExpression(b);

            }   
            if (node.Right is BoundBinaryExpression x)
            {
                node.Right = RewriteBinaryExpression(x);
            }
            if(node.Left is BoundLiteralExpression l && node.Right is BoundLiteralExpression r)
            {
                var result = node.Op.Kind switch
                {
                    //Universal
                    BoundBinaryOperatorKind.LogicalEquals => EqualityEvaluator.LeftEqualsRight(l.Value, r.Value),
                    BoundBinaryOperatorKind.NotEquals => !EqualityEvaluator.LeftEqualsRight(l.Value, r.Value),

                    //Int
                    BoundBinaryOperatorKind.Addition => Operations.Addition(l.Value, r.Value),
                    BoundBinaryOperatorKind.Subtraction => Operations.Subtraction(l.Value, r.Value),
                    BoundBinaryOperatorKind.Multiplication => Operations.Multiplication(l.Value, r.Value),
                    BoundBinaryOperatorKind.Division => Operations.Division(l.Value, r.Value),
                    BoundBinaryOperatorKind.LesserThan => Operations.LesserThan(l.Value, r.Value),
                    BoundBinaryOperatorKind.LesserThanEquals => Operations.LesserThanEquals(l.Value, r.Value),
                    BoundBinaryOperatorKind.GreaterThan => Operations.GreaterThan(l.Value, r.Value),
                    BoundBinaryOperatorKind.GreaterThanEquals => Operations.GreaterThanEquals(l.Value, r.Value),

                    BoundBinaryOperatorKind.Pow => Operations.Pow(l.Value, r.Value),

                    BoundBinaryOperatorKind.BitwiseAND => Operations.BitwiseAND(l.Value, r.Value),
                    BoundBinaryOperatorKind.BitwiseOR => Operations.BitwiseOR(l.Value, r.Value),
                    BoundBinaryOperatorKind.BitwiseXOR => Operations.BitwiseXOR(l.Value, r.Value),

                    BoundBinaryOperatorKind.LogicalAND => BinaryExpressionEvaluator.ConvertToBool(l.Value) && BinaryExpressionEvaluator.ConvertToBool(r.Value),
                    BoundBinaryOperatorKind.LogicalOR => BinaryExpressionEvaluator.ConvertToBool(l.Value) || BinaryExpressionEvaluator.ConvertToBool(r.Value),


                    //We can throw exceptions here because we've exhausted all options,
                    //and this is an internal Uranium error, should handle this more gracefully,
                    //but during the development stage, and exception will provide more info,
                    //on the stack trace :)
                    _ => throw new($"Unexpected binary operator {node.Op.Kind}"),

                };

                return new BoundLiteralExpression(result);
            }
            return base.RewriteBinaryExpression(node);
        }
    }
}
