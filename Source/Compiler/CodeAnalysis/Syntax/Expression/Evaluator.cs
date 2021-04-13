using System;

namespace Compiler.CodeAnalysis.Syntax.Expression
{
    internal class Evaluator
    {
        private readonly ExpressionSyntax _root;
        public Evaluator(ExpressionSyntax root)
        {
            _root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private int EvaluateExpression(ExpressionSyntax node)
        {
            if (node is NumberExpressionSyntax n)
            {
                return (int)n.NumberToken.Value;
            }
            if (node is BinaryExpressionSyntax b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                switch (b.OperatorToken.Kind)
                {
                    case SyntaxKind.Plus:
                        return left + right;
                    case SyntaxKind.Minus:
                        return left - right;
                    case SyntaxKind.Multiply:
                        return left * right;
                    case SyntaxKind.Divide:
                        return left / right;
                    case SyntaxKind.Pow:
                        
                        return (int)Math.Pow( (double) left, (double)right);
                    default:
                        // We can throw exceptions here because we've exhausted all options,
                        // and this is an internal compiler error, should handle this more gracefully,
                        // but during the development stage, and exception will provide more info,
                        // on the stack trace :)
                        throw new($"Unexpected binary operator {b.OperatorToken.Kind}");
                }
            }

            if (node is ParenthesizedExpressionSyntax p)
            {
                return EvaluateExpression(p.Expression);
            }

            // Same as above ^^ 
            throw new($"Unexpected node {node.Kind}");
        }
    }
}
