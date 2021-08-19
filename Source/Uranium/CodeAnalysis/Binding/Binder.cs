using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Expression;
using Uranium.CodeAnalysis.Syntax.Statement;
using Uranium.Logging;
using Uranium.CodeAnalysis.Binding.NodeKinds;
using Uranium.CodeAnalysis.Binding.Statements;
using Uranium.CodeAnalysis.Binding.Converting;
using Uranium.CodeAnalysis.Symbols;
using Uranium.CodeAnalysis.Parsing.ParserSupport.Expression;

namespace Uranium.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        public Binder(BoundScope? parent = null)
        {
            _scope = new(parent);
        }

        //Diagnostics, pretty neat not gonna lie
        private readonly DiagnosticBag _diagnostics = new();
        private BoundScope _scope;

        //Public diagnostics that nobody can edit :C
        public DiagnosticBag Diagnostics => _diagnostics;

        //It calls this method
        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope? previous, CompilationUnitSyntax syntax)
        {
            //This method allows for scope!
            var parentScope = CreateParentScopes(previous);

            //When binding to the global scope, there is no parent to the binder
            //So if parent scope is null, that's perfectly fine!
            var binder = new Binder(parentScope);

            var statement = binder.BindStatement(syntax.Statement);
            //Getting declared variables to allow us to properly report errors of already defined variables
            var variables = binder._scope.GetDeclaredVariables();
            
            //Making the diagnostics immutable allows for less potential bugs
            var diagnostics = binder.Diagnostics.ToImmutableArray();
        
            if(previous is not null)
            {
                diagnostics = diagnostics.InsertRange(0, previous.Diagnostics);
            }

            return new(previous, diagnostics, variables, statement);
        }

        private static BoundScope? CreateParentScopes(BoundGlobalScope? previous)
        {
            var stack = new Stack<BoundGlobalScope>();
            
            //Pushing our previous scopes onto the stack this way we can get them into reverse order
            while(previous is not null)
            {
                stack.Push(previous);
                previous = previous.Previous ?? null;
            }


            var parent = CreateRootScope();
            //Removing the items from stack, while also declaring variables
            while (stack.Count > 0)
            {
                previous = stack.Pop();
                var scope = new BoundScope(parent);
                for(int i = 0; i < previous.Variables.Length; i++)
                {
                    scope.TryDeclareVariable(previous.Variables[i]);
                }

                parent = scope;
            }
            return parent;
        }

        private static BoundScope CreateRootScope()
        {
            var result = new BoundScope(null);
                
            foreach(var f in BuiltInFunctions.GetAll())
            {
                if(f is null)
                {
                    continue;
                }
                result.TryDeclareFunction(f);
            }

            return result;
        }

        //Binding the Statement 
        //After making the binder, we call to bind the statement
        private BoundStatement BindStatement(StatementSyntax syntax)
            => syntax.Kind switch // Calling the correct function based off of the syntax kind and returning it's value.
            {
                //Base expressions
                SyntaxKind.BlockStatement => BindBlockStatement( (BlockStatementSyntax)syntax ),
                SyntaxKind.ExpressionStatement => BindExpressionStatement( (ExpressionStatementSyntax)syntax ),
                SyntaxKind.VariableDeclaration => BindVariableDeclaration( (VariableDeclarationSyntax)syntax ),
                SyntaxKind.IfStatement => BindIfStatement( (IfStatementSyntax)syntax ),
                SyntaxKind.WhileStatement => BindWhileStatement( (WhileStatementSyntax)syntax ),
                SyntaxKind.ForStatement => BindForStatement( (ForStatementSyntax)syntax ),
                //We can throw here because this is all that we allow for now
                //And if we get here, we've exhausted all our options
                _ => throw new($"Unexpected syntax {syntax.Kind}"),
            };

        
        //Binding the expression
        private BoundExpression BindExpression(ExpressionSyntax syntax, bool canBeVoid = false)
        {
            var result = BindExpressionInternal(syntax);
            if(!canBeVoid && result.Type == TypeSymbol.Void)
            {
                _diagnostics.ReportExpressionMustHaveValue(syntax.Span);
                return new BoundErrorExpression();
            }
            return result;
        }

        private BoundExpression BindExpressionInternal(ExpressionSyntax syntax)
            => syntax.Kind switch // Calling the correct function based off of the syntax kind and returning it's value.
            {
                //Base expressions
                SyntaxKind.BinaryExpression => BindBinaryExpression( (BinaryExpressionSyntax)syntax ),
                SyntaxKind.UnaryExpression => BindUnaryExpression( (UnaryExpressionSyntax)syntax ),
                SyntaxKind.LiteralExpression => BindLiteralExpression( (LiteralExpressionSyntax)syntax ),
                SyntaxKind.ParenthesizedExpression => BindParenthesizedExpression( (ParenthesizedExpressionSyntax)syntax ),

                //Name + Assignments
                SyntaxKind.NameExpression => BindNameExpression( (NameExpressionSyntax)syntax ),
                SyntaxKind.AssignmentExpression => BindAssignmentExpression( (AssignmentExpressionSyntax)syntax ),
                SyntaxKind.CallExpression => BindCallExpression( (CallExpressionSyntax)syntax),
                _ => throw new($"Unexpected syntax {syntax.Kind}"),
            };


        //Scoping
        private BoundStatement BindBlockStatement(BlockStatementSyntax syntax)
        {
            //Creating a new immutable array builder
            //So that we can return a BoundBlockStatement with an immutable array parameter
            var statements = ImmutableArray.CreateBuilder<BoundStatement>();

            var nextScope = new BoundScope(_scope);
            _scope = nextScope;

            //Adding each and every thing within the current syntax's statements 
            //Into the array before making it immutable
            for(int i = 0; i < syntax.Statements.Length; i++)
            {
                var statement = BindStatement(syntax.Statements[i]);
                statements.Add(statement);
            }
            _scope = _scope.Parent ?? _scope;
            return new BoundBlockStatement(statements.ToImmutable());
        }

        //Expressions
        private BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
        {
            var expression = BindExpression(syntax.Expression, true);
            return new BoundExpressionStatement(expression);
        }

        //Variable declaration
        private BoundStatement BindVariableDeclaration(VariableDeclarationSyntax syntax)
        {
            var isReadOnly = syntax.ConstKeywordToken is not null;
            var initializer = BindExpression(syntax.Initializer);
            var type = TextChecker.GetKeywordType(syntax.KeywordToken.Kind) ?? initializer.Type;
            var variable = BindVariable(syntax.Identifier, isReadOnly, type); 

            return new BoundVariableDeclaration(variable, initializer);
        }

        private BoundStatement BindIfStatement(IfStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, TypeSymbol.Bool);
            var body = BindBlockStatement(syntax.Body);
            var elseStatement = syntax.ElseClause is null ? null : BindStatement(syntax.ElseClause.ElseStatement);
            return new BoundIfStatement(condition, body, elseStatement);
        }
        
        private BoundStatement BindWhileStatement(WhileStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Expression, TypeSymbol.Bool);
            var body = BindStatement(syntax.Body);
            return new BoundWhileStatement(condition, body);
        }

        private BoundStatement BindForStatement(ForStatementSyntax syntax)
        {
            _scope = new(_scope);
            var variable = syntax.Variable is null ? null : BindStatement(syntax.Variable) as BoundVariableDeclaration;
            BoundBinaryExpression? condition;
            if(syntax.Condition is null)
            {
                condition = null;
            }
            else
            {
                var newCondition = BindExpression(syntax.Condition);
                if (newCondition is BoundBinaryExpression b)
                {
                    condition = b;
                }
                else
                {
                    condition = CreateBinaryExpressionFromForStatement(syntax);
                }
            }
            var increment = syntax.Incrementation is null ? null : BindExpression(syntax.Incrementation) as BoundAssignmentExpression;
            var block = BindBlockStatement(syntax.Body) as BoundBlockStatement;
            _scope = _scope.Parent ?? _scope;
            return new BoundForStatement(variable, condition, increment, block!);
        }

        private BoundExpression BindExpression(ExpressionSyntax syntax, TypeSymbol targetType)
        {
            var result = BindExpression(syntax);
            if (result.Type != targetType && result.Type != TypeSymbol.Bool && targetType != TypeSymbol.Bool)
            {
                _diagnostics.ReportCannotConvert(syntax.Span, result.Type, targetType);
            }
            return result;
        }

        //Value is being parsed into a nullable int
        //That then gets checked to see if it's null, and gets assigned to 0 if it is.
        private static BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax) 
            => new BoundLiteralExpression(syntax.Value);

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            var boundOperatorKind = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);

            if(boundOperand.Type == TypeSymbol.Error)
            {
                return new BoundErrorExpression();
            }
            //Checking to see if the result of our BindUnaryOperatorKind call is null
            //And reporting it to the diagnostics
            //Then returning our boundOperand
            if (boundOperatorKind is null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text ?? "null", boundOperand.Type);
                return new BoundErrorExpression();
            }

            return new BoundUnaryExpression(boundOperatorKind, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);

            var boundOperatorKind = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);

            if(boundLeft.Type == TypeSymbol.Error || boundRight.Type == TypeSymbol.Error)
            {
                return new BoundErrorExpression();
            }

            //Same as in the BindUnaryExpression but we return our boundLeft instead
            if (boundOperatorKind is null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
                return new BoundErrorExpression();
            }
            return new BoundBinaryExpression(boundLeft, boundOperatorKind, boundRight);
        }

        //Just to stay consistant tbh
        private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax) => BindExpression(syntax.Expression);

        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;

            if(string.IsNullOrEmpty(name))
            {
                return new BoundErrorExpression();
            }
            //Trying to get the value, if it returns then great, if not we report it
            if (!_scope.TryLookupVariable(name, out var variable))
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name ?? "name is null");
                return new BoundErrorExpression();
            }
            return new BoundVariableExpression(variable);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);

            if(!_scope.TryLookupVariable(name, out var variable))
            {
                //Null check on the name so that we can find the object
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
                return new BoundErrorExpression();
            }
            if(variable.IsReadOnly)
            {
                _diagnostics.ReportCannotAssign(syntax.IdentifierToken.Span, syntax.EqualsToken.Span, name);
            }

            return new BoundAssignmentExpression(variable, boundExpression, syntax.CompoundOperator, syntax.IsCompound);
        }

        private BoundExpression BindCallExpression(CallExpressionSyntax syntax)
        {
            if(syntax.Arguments.Count == 1 && LookupType(syntax.Identifier.Text) is TypeSymbol t)
            {
                return BindConversion(t, syntax.Arguments[0]);
            }

            var arguments = ImmutableArray.CreateBuilder<BoundExpression>();

            foreach(var arg in syntax.Arguments)
            {
                var boundArg = BindExpression(arg);
                arguments.Add(boundArg);
            }

            if(!_scope.TryLookupFunction(syntax.Identifier.Text, out var identifiedFunction))
            {
                _diagnostics.ReportUndefinedFunction(syntax.Identifier.Span, syntax.Identifier.Text);
                return new BoundErrorExpression();
            }

            if(syntax.Arguments.Count != identifiedFunction.Parameters.Length)
            {
                _diagnostics.ReportWrongArgumentCount(syntax.Arguments.Count, identifiedFunction.Parameters.Length, syntax.Identifier.Text, syntax.Identifier.Span);
                return new BoundErrorExpression();
            }

            for(var i = 0; i < syntax.Arguments.Count; i++)
            {
                var argument = arguments[i];
                var parameter = identifiedFunction.Parameters[i];
                
                if(argument.Type != parameter.Type)
                {
                    _diagnostics.ReportInvalidParameter(syntax.Span, identifiedFunction.Name, parameter.Name, parameter.Type, argument.Type);
                    return new BoundErrorExpression();
                }
            }
            return new BoundCallExpression(identifiedFunction, arguments.ToImmutable());
        }

        private BoundExpression BindConversion(TypeSymbol type, ExpressionSyntax syntax)
        {
            var expression = BindExpression(syntax);
            var conversion = Conversion.Classify(expression.Type, type);
            if(!conversion.Exists)
            {
                _diagnostics.ReportCannotConvert(syntax.Span, expression.Type, type);
                return new BoundErrorExpression();
            }
            return new BoundConversionExpression(type, expression);
        }

        private VariableSymbol BindVariable(SyntaxToken identifier, bool isReadOnly, TypeSymbol type)
        {
            var name = identifier.Text ?? "?";
            var canDeclare = identifier is not null;
            var variable = new VariableSymbol(name, isReadOnly, type, identifier);

            if(canDeclare && !_scope.TryDeclareVariable(variable))
            {
                _diagnostics.ReportVariableAlreadyDeclared(identifier!.Span, name);
            }
            return variable;
        }

        private BoundBinaryExpression CreateBinaryExpressionFromForStatement(ForStatementSyntax syntax)
        {
            if(syntax.Incrementation is null || syntax.Variable is null)
            {
                var boundTrueLiteral = new BoundLiteralExpression(true);
                var boundOp = new BoundBinaryOperator(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, TypeSymbol.Bool);
                return new BoundBinaryExpression(boundTrueLiteral, boundOp, boundTrueLiteral);
            }
            else
            {
                var variableDec = (VariableDeclarationSyntax)syntax.Variable;

                var initializer = BindExpression(variableDec.Initializer);

                if(!_scope.TryLookupVariable(variableDec.Identifier.Text ?? "?", out var symbol))
                {
                    _diagnostics.ReportUndefinedName(variableDec.Identifier.Span, variableDec.Identifier.Text ?? "?");
                }
                
                var variable = new BoundVariableDeclaration(symbol, initializer);
                var identifier = new BoundVariableExpression(variable!.Variable);
                var boundOp = new BoundBinaryOperator(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, TypeSymbol.Bool);
                var falseLiteral = new BoundLiteralExpression(false); 
                
                return new BoundBinaryExpression(identifier, boundOp, falseLiteral);
            }
        }

        private static TypeSymbol? LookupType(string name) => TextChecker.GetKeywordType(name);
    }
}
