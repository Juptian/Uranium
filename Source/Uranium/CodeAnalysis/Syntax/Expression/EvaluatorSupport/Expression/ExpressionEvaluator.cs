using System;
using System.Collections.Generic;
using Uranium.CodeAnalysis.Binding;
using Uranium.CodeAnalysis.Binding.Statements;
using Uranium.CodeAnalysis.Binding.NodeKinds;
using Uranium.CodeAnalysis.Text;
using Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Types;

namespace Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Expression
{
    internal static class ExpressionEvaluator
    {
        public static object EvaluateExpression(BoundExpression node, Evaluator eval)
            => node switch
            {
                //if it's a literal expression, return it's value
                BoundLiteralExpression n => n.Value,
                //if it's a Unary expression, we just evaluate the operand
                //and return it's value according to the symbol
                //Moved into it's own function because it's kinda chonky
                BoundUnaryExpression u => UnaryExpressionEvaluator.EvaluateBoundUnaryExpression(u, eval),
                BoundVariableExpression v => eval.Variables[v.Variable],
                BoundAssignmentExpression a => AssignmentEvaluator.EvaluateAssignmentExpression(a, eval),
                BoundBinaryExpression b => BinaryExpressionEvaluator.EvaluateBoundBinaryExpression(b, eval),
                _ => throw new($"Unexpected node {node.Kind}"),//Same as above ^^
            };
    }
}
