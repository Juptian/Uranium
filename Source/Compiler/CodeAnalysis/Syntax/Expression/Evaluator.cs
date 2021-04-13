using System;
using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Binding.NodeKinds;

namespace Compiler.CodeAnalysis.Syntax.Expression
{
    //Basically just the teacher that only gives
    //pop quizzes and tests
    //It just evaluates
    internal sealed class Evaluator
    {
        private readonly BoundExpression _root;
        public Evaluator(BoundExpression root)
        {
            _root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private int EvaluateExpression(BoundExpression node)
        {
            //if it's a literal expression, return it's value
            if (node is BoundLiteralExpression n) 
            {
                return (int)n.Value;
            }
            //if it's a Unary expression, we just evaluate the operand
            //and return it's value according to the symbol
            else if(node is BoundUnaryExpression u)
            {
                var operand = EvaluateExpression(u.Operand);

                if(u.OperatorKind == BoundUnaryOperatorKind.Identity)
                {
                    return operand;
                }
                else if(u.OperatorKind == BoundUnaryOperatorKind.Negation)
                {
                    return -operand;
                }

                Console.Error.WriteLine($"Unexpected unary operator {u.Kind}");
            }
            //If it's none of the above, we check out last resort
            //A BoundBinaryExpression, here we evaluate the left and right sides of both expressions
            //then return a value based off of the current operator kind
            else if (node is BoundBinaryExpression b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                switch (b.OperatorKind)
                {
                    case BoundBinaryOperatorKind.Addition:
                        return left + right;
                    case BoundBinaryOperatorKind.Subtraction:
                        return left - right;
                    case BoundBinaryOperatorKind.Multiplication:
                        return left * right;
                    case BoundBinaryOperatorKind.Division:
                        return left / right;
                    case BoundBinaryOperatorKind.Pow:
                        //This is terrible, don't do this
                        //I just haven't added support for multiple types yet, and Math.Pow only takes doubles 
                        return (int)Math.Pow( (double) left, (double)right);
                    
                    default:
                        //We can throw exceptions here because we've exhausted all options,
                        //and this is an internal compiler error, should handle this more gracefully,
                        //but during the development stage, and exception will provide more info,
                        //on the stack trace :)
                        throw new($"Unexpected binary operator {b.OperatorKind}");
                }
            }
            //Same as above ^^ 
            throw new($"Unexpected node {node.Kind}");
        }
    }
}
