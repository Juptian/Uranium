using System;
using Uranium.CodeAnalysis.Binding.NodeKinds;

namespace Uranium.CodeAnalysis.Syntax.EvaluatorSupport
{
    internal static class BinaryExpressionEvaluator
    {
        public static object EvaluateBoundBinaryExpression(BoundBinaryExpression b, Evaluator eval)
        {
            var left = ExpressionEvaluator.Evaluate(b.Left, eval);
            var right = ExpressionEvaluator.Evaluate(b.Right, eval);
            return b.Op.Kind switch
            {
                //Universal
                BoundBinaryOperatorKind.LogicalEquals => EqualityEvaluator.LeftEqualsRight(left, right),
                BoundBinaryOperatorKind.NotEquals => !EqualityEvaluator.LeftEqualsRight(left, right),

                BoundBinaryOperatorKind.Addition => Operations.Addition(left, right),
                BoundBinaryOperatorKind.Subtraction => Operations.Subtraction(left, right),
                BoundBinaryOperatorKind.Multiplication => Operations.Multiplication(left, right),
                BoundBinaryOperatorKind.Division => Operations.Division(left, right),
                BoundBinaryOperatorKind.LesserThan => Operations.LesserThan(left, right), 
                BoundBinaryOperatorKind.LesserThanEquals => Operations.LesserThanEquals(left, right),
                BoundBinaryOperatorKind.GreaterThan => Operations.GreaterThan(left, right),
                BoundBinaryOperatorKind.GreaterThanEquals => Operations.GreaterThanEquals(left, right),

                BoundBinaryOperatorKind.Pow => Operations.Pow(left, right),

                BoundBinaryOperatorKind.BitwiseAND => Operations.BitwiseAND(left, right),
                BoundBinaryOperatorKind.BitwiseOR => Operations.BitwiseOR(left, right),
                BoundBinaryOperatorKind.BitwiseXOR => Operations.BitwiseXOR(left, right),

                BoundBinaryOperatorKind.LogicalAND => ConvertToBool(left) && ConvertToBool(right),
                BoundBinaryOperatorKind.LogicalOR => ConvertToBool(left) || ConvertToBool(right),

                //We can throw exceptions here because we've exhausted all options,
                //and this is an internal Uranium error, should handle this more gracefully,
                //but during the development stage, and exception will provide more info,
                //on the stack trace :)
                _ => throw new($"Unexpected binary operator {b.Op.Kind}"),
            };
        }
        
        public static bool ConvertToBool(object obj)
        {
            return obj switch
            {
                int => (int)obj != 0,
                long => (long)obj != 0,
                float => (float)obj != 0,
                double => (double)obj != 0,
                string => (string)obj == "true" || (string)obj != "0",
                char => (char)obj != '0',
                _ => Convert.ToBoolean(obj)
            };
        }
    }
}
