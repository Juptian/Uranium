using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using Uranium.CodeAnalysis.Binding;
using Uranium.CodeAnalysis.Binding.Statements;
using Uranium.CodeAnalysis.Text;
using Uranium.CodeAnalysis.Syntax.EvaluatorSupport;
using Uranium.CodeAnalysis.Symbols;

namespace Uranium.CodeAnalysis.Syntax
{
    //Basically just the teacher that only gives
    //pop quizzes and tests
    //It just evaluates
    internal sealed class Evaluator
    {
        private readonly BoundBlockStatement _root;
        internal readonly Dictionary<VariableSymbol, object> Variables;
        private readonly Dictionary<BoundLabel, int> _labelIndex = new();

        private int _index = 0;

        internal object? LastValue;


        public Evaluator(BoundBlockStatement root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            Variables = variables;
        }

        public object Evaluate()
        {
            for(int i = 0; i < _root.Statements.Length; i++)
            {
                if(_root.Statements[i] is BoundLabelStatement l)
                {
                    _labelIndex.Add(l.Symbol, i + 1);
                }
            }

            for(; _index < _root.Statements.Length; _index++)
            {
                EvaluateStatement(_root.Statements[_index]);
            }

            return LastValue!;
        }

        public void EvaluateStatement(BoundStatement statement)
        {
            switch (statement.Kind)
            {
                case BoundNodeKind.ExpressionStatement:
                    EvaluateExpressionStatement((BoundExpressionStatement)statement);

                    return;
                case BoundNodeKind.VariableDeclaration:
                    VariableDeclarationEvaluator.Evaluate((BoundVariableDeclaration)statement, this);
                    return;

                case BoundNodeKind.LabelStatement:
                    return;

                case BoundNodeKind.GotoStatement:
                    var gs = (BoundGotoStatement)statement;
                    _index = _labelIndex[gs.Label] - 1;

                    return;
                case BoundNodeKind.ConditionalGotoStatement:
                    var cgs = (BoundConditionalGotoStatement)statement;
                    var condition = ExpressionEvaluator.Evaluate(cgs.Condition, this);
                    if(condition is not bool b)
                    {
                        b = BinaryExpressionEvaluator.ConvertToBool(condition);
                    }
                    if(b && !cgs.JumpIfFalse ||
                       !b && cgs.JumpIfFalse)
                    {
                        _index = _labelIndex[cgs.Label] - 1;
                    }
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
