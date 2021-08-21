using System;
using System.Collections.Immutable;
using System.Linq;
using Uranium.CodeAnalysis.Binding.Statements;
using Uranium.CodeAnalysis.Binding.NodeKinds;
using Uranium.CodeAnalysis.Binding;

namespace Uranium.CodeAnalysis.Lowering
{
    internal abstract class BoundTreeRewriter
    {

        public virtual BoundStatement RewriteStatement(BoundStatement node)
        {
            return node.Kind switch
            {
                BoundNodeKind.BlockStatement => RewriteBlockStatement( (BoundBlockStatement)node),
                BoundNodeKind.ExpressionStatement => RewriteExpressionStatement( (BoundExpressionStatement)node),
                BoundNodeKind.VariableDeclaration => RewriteVariableDeclaration( (BoundVariableDeclaration)node),
                BoundNodeKind.IfStatement => RewriteIfStatement( (BoundIfStatement) node),
                BoundNodeKind.WhileStatement => RewriteWhileStatement( (BoundWhileStatement)node),
                BoundNodeKind.ForStatement => RewriteForStatement( (BoundForStatement)node),
                BoundNodeKind.LabelStatement => RewriteLabelStatement( (BoundLabelStatement)node ),
                BoundNodeKind.GotoStatement => RewriteGotoStatement( (BoundGotoStatement)node ),
                BoundNodeKind.ConditionalGotoStatement => RewriteConditionalGotoStatement( (BoundConditionalGotoStatement)node ),

                _ => throw new($"Unexpected node: {node.Kind}"),
            };
        }


        public virtual BoundExpression RewriteExpression(BoundExpression node)
        {

            return node.Kind switch
            {
                BoundNodeKind.ErrorExpression => RewriteErrorExpression((BoundErrorExpression)node),
                BoundNodeKind.UnaryExpression => RewriteUnaryExpression((BoundUnaryExpression)node),
                BoundNodeKind.LiteralExpression => RewriteLiteralExpression((BoundLiteralExpression)node),
                BoundNodeKind.BinaryExpression => RewriteBinaryExpression((BoundBinaryExpression)node),
                BoundNodeKind.VariableExpression => RewriteVariableExpression((BoundVariableExpression)node),
                BoundNodeKind.AssignmentExpression => RewriteAssignmentExpression((BoundAssignmentExpression)node),
                BoundNodeKind.CallExpression => RewriteCallExpression((BoundCallExpression)node),
                BoundNodeKind.ConversionExpression => RewriteConversionExpression((BoundConversionExpression)node),
                _ => throw new($"Unexpected node: {node.Kind}"),
            };
        }

        protected virtual BoundStatement RewriteBlockStatement(BoundBlockStatement node)
        {
            ImmutableArray<BoundStatement>.Builder? builder = null;
            for(var i = 0; i < node.Statements.Length; i++)
            {
                var oldStatement = node.Statements[i];
                var newStatement = RewriteStatement(oldStatement);
                if(newStatement != oldStatement)
                {
                    if(builder is null)
                    {
                        builder = ImmutableArray.CreateBuilder<BoundStatement>(node.Statements.Length);
                        for(var j = 0; j < i; j++)
                        {
                            builder.Add(node.Statements[j]);
                        }
                    }
                }
                if(builder is not null)
                {
                    builder.Add(newStatement);
                }
            }

            if(builder is null)
            {
                return node;
            }

            return new BoundBlockStatement(builder.MoveToImmutable());
        }

        protected virtual BoundStatement RewriteExpressionStatement(BoundExpressionStatement node)
        {
            var expression = RewriteExpression(node.Expression);
            if(expression == node.Expression)
            {
                return node;
            }

            return new BoundExpressionStatement(expression);
        }

        protected virtual BoundStatement RewriteVariableDeclaration(BoundVariableDeclaration node)
        {
            if(node.Initializer is null)
            {
                return node;
            }
            var initializer = RewriteExpression(node.Initializer);
            if(initializer == node.Initializer)
            {
                return node;
            }

            return new BoundVariableDeclaration(node.Variable, initializer);
        }

        protected virtual BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            var condition = RewriteExpression(node.Condition);
            var body = RewriteStatement(node.Statement);
            var elseClause = node.ElseStatement is null ? null : RewriteStatement(node.ElseStatement);
            if(condition == node.Condition && body == node.Statement && elseClause == node.ElseStatement)
            {
                return node;
            }

            return new BoundIfStatement(condition, body, elseClause);
        }

        protected virtual BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            var condition = RewriteExpression(node.Condition);
            var body = RewriteStatement(node.Body) as BoundBlockStatement;
            if(condition == node.Condition && body == node.Body)
            {
                return node;
            }
            return new BoundWhileStatement(condition, body ?? node.Body);
        }

        protected virtual BoundStatement RewriteForStatement(BoundForStatement node)
        {
            BoundVariableDeclaration? initializer = null;
            BoundBinaryExpression? condition = null;
            BoundAssignmentExpression? incrementation = null;

            if(node.VariableDeclaration is not null)
            {
                initializer = (BoundVariableDeclaration)RewriteStatement(node.VariableDeclaration);
            }

            if(node.Condition is not null)
            {
                condition = (BoundBinaryExpression)RewriteExpression(node.Condition);
            }

            if(node.Increment is not null)
            {
                incrementation = (BoundAssignmentExpression)RewriteExpression(node.Increment);
            }

            if(initializer is null && condition is null && incrementation is null)
            {
                var trueExpression = new BoundLiteralExpression(true);
                return new BoundWhileStatement(trueExpression, node.Body);
            }

            var body = RewriteBlockStatement(node.Body) as BoundBlockStatement;
            
            if(initializer == node.VariableDeclaration &&
               condition == node.Condition &&
               incrementation == node.Increment &&
               body == node.Body)
            {
                return node;
            }

            return new BoundForStatement(initializer, condition, incrementation, body ?? node.Body);
        }

        protected virtual BoundStatement RewriteLabelStatement(BoundLabelStatement node)
            => node;

        protected virtual BoundStatement RewriteGotoStatement(BoundGotoStatement node)
            => node;

        protected virtual BoundStatement RewriteConditionalGotoStatement(BoundConditionalGotoStatement node)
        {
            var condition = RewriteExpression(node.Condition);
            if(condition == node.Condition)
            {
                return node;
            }

            return new BoundConditionalGotoStatement(node.Label, condition, node.JumpIfFalse);
        }

        protected virtual BoundExpression RewriteErrorExpression(BoundErrorExpression node)
            => node;

        protected virtual BoundExpression RewriteUnaryExpression(BoundUnaryExpression node)
        {
            var operand = RewriteExpression(node.Operand);
            if(operand == node.Operand)
                return node;
            return new BoundUnaryExpression(node.Op, operand);
        }

        protected virtual BoundExpression RewriteLiteralExpression(BoundLiteralExpression node)
            => node;

        protected virtual BoundExpression RewriteBinaryExpression(BoundBinaryExpression node)
        {
            var left = RewriteExpression(node.Left);
            var right = RewriteExpression(node.Right);
            if(left == node.Left && right == node.Right)
            {
                return node;
            }
            return new BoundBinaryExpression(left, node.Op, right);
        }

        protected virtual BoundExpression RewriteVariableExpression(BoundVariableExpression node)
            => node;

        protected virtual BoundExpression RewriteAssignmentExpression(BoundAssignmentExpression node)
        {
            var expression = RewriteExpression(node.Expression);
            if(expression == node.Expression)
            {
                return node;
            }
            return new BoundAssignmentExpression(node.Variable, expression, node.CompoundOperator, node.IsCompound);
        }

        protected virtual BoundExpression RewriteCallExpression(BoundCallExpression node)
        {
            ImmutableArray<BoundExpression>.Builder? builder = null;
            for(var i = 0; i < node.Arguments.Length; i++)
            {
                var oldArgument = node.Arguments[i];
                var newArgument = RewriteExpression(oldArgument);
                if(newArgument != oldArgument)
                {
                    if(builder is null)
                    {
                        builder = ImmutableArray.CreateBuilder<BoundExpression>(node.Arguments.Length);
                        for(var j = 0; j < i; j++)
                        {
                            builder.Add(node.Arguments[j]);
                        }
                    }
                }
                if(builder is not null)
                {
                    builder.Add(newArgument);
                }
            }

            if(builder is null)
            {
                return node;
            }

            return new BoundCallExpression(node.Function, builder.MoveToImmutable());
        }
        
        protected virtual BoundExpression RewriteConversionExpression(BoundConversionExpression node)
        {
            var expression = RewriteExpression(node.Expression);
            if(expression == node.Expression)
            {
                return node;
            }

            return new BoundConversionExpression(node.Type, expression);
        }
    }
}
