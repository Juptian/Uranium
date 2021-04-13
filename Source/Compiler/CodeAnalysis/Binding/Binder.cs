using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.CodeAnalysis.Syntax;
using Compiler.CodeAnalysis.Binding.NodeKinds;
using Compiler.CodeAnalysis.Syntax.Expression;

namespace Compiler.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        //Binding the expression
        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            //Calling the correct function based off of the syntax kind
            //Also returning it's value
            switch(syntax.Kind)
            {
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);
                
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);

                default:
                    throw new($"Unexpected syntax {syntax.Kind}");
            }
        }

        //Diagnostics, pretty neat not gonna lie
        private readonly List<string> _diagnostics = new();

        //Public diagnostics that nobody can edit :C
        public IEnumerable<string> Diagnostics => _diagnostics;

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            //Value is being parsed into a nullable int
            //That then gets checked to see if it's null, and gets assigned to 0 if it is.
            var value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            var boundOperatorKind = BindUnaryOperatorKind(syntax.OperatorToken.Kind, boundOperand.Type);
             Console.WriteLine(boundOperatorKind);
            Console.WriteLine(boundOperatorKind is null);
           
            //Checking to see if the result of our BindUnaryOperatorKind call is null
            //And reporting it to the diagnostics
            //Then returning our boundOperand
            if(boundOperatorKind is null)
            {
                Console.WriteLine(_diagnostics.Count);
                _diagnostics.Add($"Unary operand {syntax.OperatorToken.Text} is not defined for {boundOperand.Type}!");
                Console.WriteLine(_diagnostics.Count);
                return boundOperand;
            }
            
            return new BoundUnaryExpression(boundOperatorKind.Value, boundOperand);
        }


        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);

            var boundOperatorKind = BindBinaryOperatorKind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);
             Console.WriteLine(boundOperatorKind);
           
            //Same as in the BindUnaryExpression but we return our boundLeft instead
            if(boundOperatorKind == null)
            {
                _diagnostics.Add($"Binary operand {syntax.OperatorToken.Text} is not defined for {boundLeft.Type}!");
                return boundLeft;
            }

            return new BoundBinaryExpression(boundLeft, boundOperatorKind.Value, boundRight);
        }

        private BoundUnaryOperatorKind? BindUnaryOperatorKind(SyntaxKind kind, Type operandType)
        {
            //We only allow ints for now
            if (operandType != typeof(int))
                return null;
            // If the unary operator is not a + or - we throw an error
            // Because you can't just do /10 for example, you need another expression
            switch(kind)
            {
                case SyntaxKind.Plus:
                    return BoundUnaryOperatorKind.Identity;
                case SyntaxKind.Minus:
                    return BoundUnaryOperatorKind.Negation;
                default:
                    throw new($"Unexpected unary operator {kind}");
            }           
        }

        private BoundBinaryOperatorKind? BindBinaryOperatorKind(SyntaxKind kind, Type leftType, Type rightType)
        {
            //Returning null if right or left is not an int, we only accept an int right now
            if(leftType != typeof(int) || rightType != typeof(int))
            {
                return null;
            }
            
            //If we get pass the null check, we check what operator is used
            //Then we will use it in the evaluator according to the result
            switch(kind)
            {
                case SyntaxKind.Plus:
                    return BoundBinaryOperatorKind.Addition;
                case SyntaxKind.Minus:
                    return BoundBinaryOperatorKind.Subtraction;
                case SyntaxKind.Multiply:
                    return BoundBinaryOperatorKind.Multiplication;
                case SyntaxKind.Divide:
                    return BoundBinaryOperatorKind.Division;
                case SyntaxKind.Pow:
                    return BoundBinaryOperatorKind.Pow;

                default:
                    throw new($"Unexpected binary operator {kind}");
            }              
        }


    }
}
