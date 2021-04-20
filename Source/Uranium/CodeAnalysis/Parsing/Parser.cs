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
            
            _tokens = tokens.ToArray();
            //Doing this to keep all our previous diagnostics
            //So that we don't lose them
            _diagnostics.Concat(lexer.Diagnostics);
        }

        private SyntaxToken Current => Peek(0);
        private SyntaxToken Next => Peek(1);
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
            var EndOfFileToken = MatchToken(SyntaxKind.EndOfFile);

            return new(statement, EndOfFileToken);
        }

        //If it doesn't fit any of our current conditions, we take it as an expression
        private StatementSyntax ParseStatement()
            => Current.Kind switch
            {
                SyntaxKind.OpenCurlyBrace => ParseBlockStatement(),
                SyntaxKind.LetConstKeyword or 
                SyntaxKind.ConstKeyword or 
                SyntaxKind.VarKeyword => ParseVariableDeclaration(),
                _ => ParseExpressionStatement(),
            };

        private BlockStatementSyntax ParseBlockStatement()
        {
            var statements = ImmutableArray.CreateBuilder<StatementSyntax>();
            var openBraceToken = MatchToken(SyntaxKind.OpenCurlyBrace);

            while(Current.Kind is not SyntaxKind.EndOfFile &&
                  Current.Kind is not SyntaxKind.CloseCurlyBrace)
            {
                if(Current.Kind is SyntaxKind.LineBreak || Current.Kind is SyntaxKind.Semicolon)
                {
                    _position++;
                    continue;
                }

                var statement = ParseStatement();
                statements.Add(statement);
            }

            var closeBraceToken = MatchToken(SyntaxKind.CloseCurlyBrace);

            return new(openBraceToken, statements.ToImmutable(), closeBraceToken);
        }

        private StatementSyntax ParseVariableDeclaration()
        {
            //Could remove expected and inline it, but I find this is cleaner
            var expected = Current.Kind;
            var keyword = MatchToken(expected);
            var identifier = MatchToken(SyntaxKind.IdentifierToken);
            var equals = MatchToken(SyntaxKind.Equals);
            var initializer = ParseExpression();
            return new VariableDeclarationSyntax(keyword, identifier, equals, initializer);
        }

        private ExpressionStatementSyntax ParseExpressionStatement() => new(ParseExpression());

        //This is just another wrapper function to go through our heirarchy
        //Debating removing this but it keeps the code clear imo
        private ExpressionSyntax ParseExpression() => ParseAssignmentExpression();

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

            //If it's an identifier, we parse it as one, if not, we go to our next level
            //Binary expressions
            if(Current.Kind == SyntaxKind.IdentifierToken &&
                Next.Kind == SyntaxKind.Equals)
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

            while(true)
            {
                var precedence = Current.Kind.GetBinaryOperatorPrecedence();

                if(precedence == 0 || precedence <= parentPrecedence)
                {
                    break;
                }
                
                var operatorToken = NextToken();
                var right = ParseBinaryExpression(precedence);
                
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }
            //Console.WriteLine(left);
            return left;
        }


        private ExpressionSyntax ParsePrimaryExpression()
            =>  Current.Kind switch
            {
                SyntaxKind.OpenParenthesis => ParseParenthesizedExpression(),
                SyntaxKind.TrueKeyword or SyntaxKind.FalseKeyword => ParseBooleanExpression(),
                SyntaxKind.NumberToken => ParseNumberLiteral(),
                _ => ParseNameExpression(),
            };

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
