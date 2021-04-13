using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Syntax.Expression
{
    class Evaluator
    {
        private readonly ExpressionSyntax _root;
        public Evaluator(ExpressionSyntax root)
        {
            _root = root;
        }

        public long Evaluate()
        {
            return EvaluateExpression(_root);
        }
        private int EvaluateExpression(ExpressionSyntax node)
        {
            if(node is NumberExpressionSyntax n)
            {
                return (int)n.NumberToken.Value;
            }
            if(node is BinaryExpressionSyntax b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                switch(b.OperatorToken.Kind)
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
                        long result = left;
                        for(long i = 0; i < right; i++)
                        {
                            result *= right;
                        }
                        return (int)result;
                    default:
                        Console.Error.WriteLine($"Unexpected binary operator {b.OperatorToken.Kind}");
                        return 1;
                }
            }

            if (node is ParenthesizedExpressionSyntax p)
                return EvaluateExpression(p.Expression);
            Console.Error.WriteLine($"Unexpected node {node.Kind}");
            return 0;
        }
    }
}
