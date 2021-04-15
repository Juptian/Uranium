using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.CodeAnalysis.Syntax;
using Compiler.CodeAnalysis.Binding.NodeKinds;
using Compiler.CodeAnalysis.Syntax.Expression;
using Compiler.CodeAnalysis.Text;
using Compiler.Logging;

namespace Compiler.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        //Binding the expression
        public BoundExpression BindExpression(ExpressionSyntax syntax)
            => syntax.Kind switch // Calling the correct function based off of the syntax kind and returning it's value.
            {
                //Base expressions
                SyntaxKind.BinaryExpression => BindBinaryExpression((BinaryExpressionSyntax)syntax),
                SyntaxKind.UnaryExpression => BindUnaryExpression((UnaryExpressionSyntax)syntax),
                SyntaxKind.LiteralExpression => BindLiteralExpression((LiteralExpressionSyntax)syntax),
                SyntaxKind.ParenthesizedExpression => BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax),

                //Name + Assignments
                SyntaxKind.NameExpression => BindNameExpression((NameExpressionSyntax)syntax),
                SyntaxKind.AssignmentExpression => BindAssignmentExpression((AssignmentExpressionSyntax)syntax),
                _ => throw new($"Unexpected syntax {syntax.Kind}"),
            };

        //Diagnostics, pretty neat not gonna lie
        private readonly DiagnosticBag _diagnostics = new();
        private readonly Dictionary<VariableSymbol, object> _variables;

        public Binder(Dictionary<VariableSymbol, object> variables)
        {
            _variables = variables;
        }

        //Public diagnostics that nobody can edit :C
        public DiagnosticBag Diagnostics => _diagnostics;


        //Value is being parsed into a nullable int
        //That then gets checked to see if it's null, and gets assigned to 0 if it is.
        private static BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax) => new BoundLiteralExpression(syntax.Value ?? 0);

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            var boundOperatorKind = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);

            //Checking to see if the result of our BindUnaryOperatorKind call is null
            //And reporting it to the diagnostics
            //Then returning our boundOperand
            if (boundOperatorKind is null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text ?? "null", boundOperand.Type);
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
            if (boundOperatorKind is null)
            {
                Console.WriteLine(syntax.OperatorToken.Text);
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
                return boundLeft;
            }

            return new BoundBinaryExpression(boundLeft, boundOperatorKind, boundRight);
        }

        //Just to stay consistant tbh
        private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax) => BindExpression(syntax.Expression);

        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;

            var variables = _variables.Keys.FirstOrDefault(v => v.Name == name);

            //Trying to get the value, if it returns then great, if not we report it
            if (variables is null)
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name ?? "name is null");
                return new BoundLiteralExpression(0);
            }

            return new BoundVariableExpression(variables);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);

            var existingVariable = _variables.Keys.FirstOrDefault(v => v.Name == name);

            //removing existing variables
            if(existingVariable is not null)
            {
                _variables.Remove(existingVariable);
            }

            //Null check on the name so that we can find the object
            var variable = new VariableSymbol(name ?? $"{syntax.IdentifierToken}s name is null", boundExpression.Type);

            return new BoundAssignmentExpression(variable, boundExpression);
        }
    }
}
