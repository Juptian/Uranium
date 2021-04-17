using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Uranium.CodeAnalysis.Lexing;
using Uranium.CodeAnalysis.Syntax.Expression;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Statement;
using Uranium.CodeAnalysis.Text;
using Uranium.Logging;

namespace Uranium.CodeAnalysis.Parsing
{
    internal sealed class Parser
    {
        private readonly SyntaxToken[] _tokens;
        private int _position;
        private readonly SourceText _text;
        private readonly DiagnosticBag _diagnostics = new();

        public Parser(SourceText text)
        {
            var tokens = new List<SyntaxToken>();
            var lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.Lex();
                if (token.Kind is not SyntaxKind.WhiteSpace &&  token.Kind is not SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            }
            while (token.Kind is not SyntaxKind.EndOfFile);

            _text = text;
            _tokens = tokens.ToArray();
            _diagnostics.Concat(lexer.Diagnostics);
        }

        private SyntaxToken Current => Peek(0);
        public DiagnosticBag Diagnostics => _diagnostics;
        
        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _tokens.Length)
            {
                return _tokens[^1];
            }

            return _tokens[index];
        }

        private SyntaxToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        private SyntaxToken MatchToken(SyntaxKind kind)
        {
            if (Current.Kind == kind)
            {
                return NextToken();
            }

            _diagnostics.ReportInvalidToken(Current.Span, Current, kind);

            return new(kind, Current.Position, Current.Text, null);
        }

        public CompilationUnitSyntax ParseCompilationUnit()
        {
            var statement = ParseStatement();
            var EOFToken = MatchToken(SyntaxKind.EndOfFile);

            return new(statement, EOFToken);
        }

        private StatementSyntax ParseStatement()
        {
            if(Current.Kind is SyntaxKind.OpenCurlyBrace)
            {
                return ParseBlockStatement();
            }
            return ParseExpressionStatement();
        }

        private ExpressionStatementSyntax ParseExpressionStatement() => new(ParseExpression());

        private BlockStatementSyntax ParseBlockStatement()
        {
            var statements = ImmutableArray.CreateBuilder<StatementSyntax>();
            var openBraceToken = MatchToken(SyntaxKind.OpenCurlyBrace);

            while(Current.Kind is not SyntaxKind.EndOfFile &&
                  Current.Kind is not SyntaxKind.CloseCurlyBrace)
            {
                var statement = ParseStatement();
                statements.Add(statement);
            }

            var closeBraceToken = MatchToken(SyntaxKind.CloseCurlyBrace);

            return new(openBraceToken, statements.ToImmutable(), closeBraceToken);
        }

        private ExpressionSyntax ParseExpression()
        {
            return ParseAssignmentExpression();
        }

        private ExpressionSyntax ParseAssignmentExpression()
        {
            //Assignments will be done like this:
            //
            //a = b = 5
            //
            //
            //   =
            //  / \
            // a   =
            //    / \
            //    b  5
            //Checking for an identifier token, and a double equals
            //this way we can actually assign two identifiers
            
            if(Current.Kind == SyntaxKind.IdentifierToken &&
                Peek(1).Kind == SyntaxKind.Equals)
            {
                //Despite knowing the token, we want to consume it, to avoid loops
                var identifierToken = NextToken();
                var operatorToken = NextToken();

                var right = ParseAssignmentExpression();
                return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
            }
            return ParseBinaryExpression();
        }

        //Allowing for proper operator precedence
        private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
        {

            ExpressionSyntax left;
            var unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();

            //Allowing for unary operator precedence
            if(unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
            {
                var operatorToken = NextToken();
                var operand = ParseBinaryExpression(unaryOperatorPrecedence);
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }
            
            //Keep looping until our precedence is <= parent precedence, or == 0;
            while(true)
            {
                var precedence = Current.Kind.GetBinaryOperatorPrecedence();
                
                if(precedence == 0 || precedence <= parentPrecedence)
                {
                    break;
                }
                
                //Taking the current token, and moving the index
                var operatorToken = NextToken();
                //Recursively calling the ParseExpression with the current precedence
                var right = ParseBinaryExpression(precedence);

                //Making left a New BinaryExpressionSyntax
                left = new BinaryExpressionSyntax(left, operatorToken, right);

            }
            return left;
        }


        private ExpressionSyntax ParsePrimaryExpression()
        {
            //Converted to switch before we get too many checks
            return Current.Kind switch
            {
                //All extracted into methods for the sake of readability, and reuseability.
                //Parenthesis
                SyntaxKind.OpenParenthesis => ParseParenthesizedExpression(),
                //Bools
                SyntaxKind.TrueKeyword or SyntaxKind.FalseKeyword => ParseBooleanExpression(),
                //Identifiers
                SyntaxKind.NumberToken => ParseNumberLiteral(),
                //Assuming default is a Name expression
                _ => ParseNameExpression(),//^^
            };
        }

        private ExpressionSyntax ParseParenthesizedExpression()
        {
            var left = MatchToken(SyntaxKind.OpenParenthesis);
            var expression = ParseExpression();
            var right = MatchToken(SyntaxKind.CloseParenthesis);
            return new ParenthesizedExpressionSyntax(left, expression, right);
        }

        private ExpressionSyntax ParseBooleanExpression()
        {
            var isTrue = Current.Kind == SyntaxKind.TrueKeyword;
            var keywordToken = isTrue ? MatchToken(SyntaxKind.TrueKeyword) : MatchToken(SyntaxKind.FalseKeyword);
            return new LiteralExpressionSyntax(keywordToken, isTrue);
        }

        private ExpressionSyntax ParseNumberLiteral()
        {
            var numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }

        private ExpressionSyntax ParseNameExpression()
        {
            var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
            return new NameExpressionSyntax(identifierToken);
        }

    }
}
