using System;
using System.Collections.Generic;
using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Binding.NodeKinds;
using Compiler.CodeAnalysis.Text;

namespace Compiler.CodeAnalysis.Syntax.Expression
{
    //Basically just the teacher that only gives
    //pop quizzes and tests
    //It just evaluates
    internal sealed class Evaluator
    {
        private readonly BoundExpression _root;
        private readonly Dictionary<VariableSymbol, object> _variables;
        public Evaluator(BoundExpression root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private object EvaluateExpression(BoundExpression node)
        {
            switch(node)
            {
                //if it's a literal expression, return it's value
                case BoundLiteralExpression n: 
                    return n.Value;

                //if it's a Unary expression, we just evaluate the operand
                //and return it's value according to the symbol
                case BoundUnaryExpression u:
                   var operand = EvaluateExpression(u.Operand);
                    switch(u.Op.Kind)
                    {
                        case BoundUnaryOperatorKind.Identity:
                            return (int)operand;
                        case BoundUnaryOperatorKind.Negation:
                            return -(int)operand;
                        case BoundUnaryOperatorKind.LogicalNegation:
                            return !(bool)operand;
                    }

                    Console.Error.WriteLine($"Unexpected unary operator {u.Kind}");
                    break;

                case BoundVariableExpression v:
                    return _variables[v.Variable];

                case BoundAssignmentExpression a:
                    var value = EvaluateExpression(a.Expression);
                    _variables[a.Variable] = value;
                    return value;

                //If it's none of the above, we check out last resort
                //A BoundBinaryExpression, here we evaluate the left and right sides of both expressions
                //then return a value based off of the current operator kind
                case BoundBinaryExpression b:
                     var left = EvaluateExpression(b.Left);
                    var right = EvaluateExpression(b.Right);
                        
                    switch (b.Op.Kind)  
                    {
                        //Universal
                        case BoundBinaryOperatorKind.LogicalEquals:
                            return Equals(left, right); 
                        case BoundBinaryOperatorKind.NotEquals:
                            return !Equals(left, right);

                        //Int
                        case BoundBinaryOperatorKind.Addition:
                            return (int)left + (int)right;
                        case BoundBinaryOperatorKind.Subtraction:
                            return (int)left - (int)right;
                        case BoundBinaryOperatorKind.Multiplication:
                            return (int)left * (int)right;
                        case BoundBinaryOperatorKind.Division:
                            return (int)left / (int)right;
                        case BoundBinaryOperatorKind.Pow:
                            var result = (int)left;
                            var leftAsInt = (int)left;
                            for(var i = 1; i < (int)right; i++)
                            {
                                result *= leftAsInt;
                            }
                            return result;
                                            
                        //Bool
                        case BoundBinaryOperatorKind.LogicalAND:
                            return (bool)left && (bool)right;
                        case BoundBinaryOperatorKind.LogicalOR:
                            return (bool)left || (bool)right;
                        case BoundBinaryOperatorKind.LogicalXOREquals:
                            var leftBool = (bool)left;
                            var rightBool = (bool)right;
                            return leftBool ^= rightBool;
                        case BoundBinaryOperatorKind.LogicalXOR:
                            return (bool)left ^ (bool)right;

                    
                        default:
                            //We can throw exceptions here because we've exhausted all options,
                            //and this is an internal compiler error, should handle this more gracefully,
                            //but during the development stage, and exception will provide more info,
                            //on the stack trace :)
                            throw new($"Unexpected binary operator {b.Op.Kind}");
                    }
                default:
                    //Same as above ^^
                    throw new($"Unexpected node {node.Kind}");
            }
            //Same as above ^^
            throw new($"Unexpected node {node.Kind}");
        }
    }
}
