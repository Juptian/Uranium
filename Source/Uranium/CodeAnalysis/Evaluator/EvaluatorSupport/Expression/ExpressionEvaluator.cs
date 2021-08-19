using System;
using Uranium.CodeAnalysis.Binding.NodeKinds;

namespace Uranium.CodeAnalysis.Syntax.EvaluatorSupport
{
    internal static class ExpressionEvaluator
    {
        public static object? Evaluate(BoundExpression node, Evaluator eval)
            => node switch
            {
                //if it's a literal expression, return it's value
                BoundLiteralExpression n => n.Value,

                BoundUnaryExpression u => UnaryExpressionEvaluator.EvaluateBoundUnaryExpression(u, eval),
                BoundBinaryExpression b => BinaryExpressionEvaluator.EvaluateBoundBinaryExpression(b, eval),
                
                BoundVariableExpression v => eval.Variables[v.Variable],
                BoundAssignmentExpression a => AssignmentEvaluator.EvaluateAssignmentExpression(a, eval),
                BoundCallExpression c => CallExpressionEvaluator.Evaluate(c, eval),
                _ => throw new($"Unexpected node {node.Kind}"),
            };
    }
}
