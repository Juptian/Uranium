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
            => syntax.Kind switch // Calling the correct function based off of the syntax kind and returning it's value.
            {
                SyntaxKind.BinaryExpression => BindBinaryExpression((BinaryExpressionSyntax)syntax),
                SyntaxKind.UnaryExpression => BindUnaryExpression((UnaryExpressionSyntax)syntax),
                SyntaxKind.LiteralExpression => BindLiteralExpression((LiteralExpressionSyntax)syntax),
                _ => throw new($"Unexpected syntax {syntax.Kind}"),
            };

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
            var boundOperatorKind = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);
           
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
            
            return new BoundUnaryExpression(boundOperatorKind, boundOperand);
        }


        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);

            var boundOperatorKind = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);
           
            //Same as in the BindUnaryExpression but we return our boundLeft instead
            if(boundOperatorKind == null)
            {
                _diagnostics.Add($"Binary operand '{syntax.OperatorToken.Kind}' is not defined for {boundLeft.Type}!");
                return boundLeft;
            }

            return new BoundBinaryExpression(boundLeft, boundOperatorKind, boundRight);
        }

        private BoundUnaryOperatorKind? BindUnaryOperatorKind(SyntaxKind kind, Type operandType)
        {
            //We only allow ints for now
            if (operandType != typeof(int))
                return null;
            // If the unary operator is not a + or - we throw an error
            // Because you can't just do /10 for example, you need another expression
            return kind switch
            {
                SyntaxKind.Plus => BoundUnaryOperatorKind.Identity,
                SyntaxKind.Minus => BoundUnaryOperatorKind.Negation,
                _ => throw new($"Unexpected unary operator {kind}"),
            };
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
            return kind switch
            {
                SyntaxKind.Plus => BoundBinaryOperatorKind.Addition,
                SyntaxKind.Minus => BoundBinaryOperatorKind.Subtraction,
                SyntaxKind.Multiply => BoundBinaryOperatorKind.Multiplication,
                SyntaxKind.Divide => BoundBinaryOperatorKind.Division,
                SyntaxKind.Pow => BoundBinaryOperatorKind.Pow,
                _ => throw new($"Unexpected binary operator {kind}"),
            };
        }
    }
}
