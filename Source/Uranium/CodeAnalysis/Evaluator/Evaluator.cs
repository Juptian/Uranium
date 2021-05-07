using System;
using System.Collections.Generic;
using Uranium.CodeAnalysis.Binding;
using Uranium.CodeAnalysis.Binding.Statements;
using Uranium.CodeAnalysis.Text;
using Uranium.CodeAnalysis.Syntax.EvaluatorSupport;

namespace Uranium.CodeAnalysis.Syntax
{
    //Basically just the teacher that only gives
    //pop quizzes and tests
    //It just evaluates
    internal sealed class Evaluator
    {
        private readonly BoundStatement _root;
        internal readonly Dictionary<VariableSymbol, object> Variables;

        internal object? LastValue;

        public Evaluator(BoundStatement root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            Variables = variables;
        }

        public object Evaluate()
        {
            EvaluateStatement(_root);
            return LastValue!;
        }

        public void EvaluateStatement(BoundStatement statement)
        {
            switch (statement.Kind)
            {
                case BoundNodeKind.BlockStatement:
                    BlockStatementEvaluator.Evaluate((BoundBlockStatement)statement, this);
                    return;

                case BoundNodeKind.ExpressionStatement:
                    EvaluateExpressionStatement((BoundExpressionStatement)statement);
                    return;
                case BoundNodeKind.VariableDeclaration:
                    VariableDeclarationEvaluator.Evaluate((BoundVariableDeclaration)statement, this);
                    return;

                case BoundNodeKind.IfStatement:
                    IfStatementEvaluator.Evaluate((BoundIfStatement)statement, this);
                    return;

                case BoundNodeKind.WhileStatement:
                    WhileStatementEvaluator.Evaluate((BoundWhileStatement)statement, this);
                    return;

                case BoundNodeKind.ForStatement:
                    ForStatementEvaluator.Evaluate((BoundForStatement)statement, this);
                    return;
                default:
                    //Exhausted all our options, time to call it quits!
                    throw new($"Unexpected statement {statement}");
            }
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement statement) 
            => LastValue = ExpressionEvaluator.Evaluate(statement.Expression, this);
    }
}
