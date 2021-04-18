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
        //private readonly SourceText _text;
        private readonly DiagnosticBag _diagnostics = new();

        //Then this is called
        public Parser(SourceText text)
        {
            //Then we make a list of tokens so that we can add our lexed tokens into it
            var tokens = new List<SyntaxToken>();
            var lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                //We lex the current index in the lexer
                token = lexer.Lex();
                //We ignore the bad tokens and whitespace as it's not needed
                if (token.Kind is not SyntaxKind.WhiteSpace &&  token.Kind is not SyntaxKind.BadToken)
                {
                    //Then we add it if it's none of the above
                    tokens.Add(token);
                }
                //Console.WriteLine(token);
            }
            while (token.Kind is not SyntaxKind.EndOfFile);
            //We repeat the lexing until we hit the end of the file
            //_text = text;
            //Then we put all the tokens into an array
            _tokens = tokens.ToArray();
            //And take the lexers diagnostics, and add them to our current ones
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

        //Which just runs this method
        public CompilationUnitSyntax ParseCompilationUnit()
        {
            //Which then calls to parse the current statement
            var statement = ParseStatement();
            var EOFToken = MatchToken(SyntaxKind.EndOfFile);

            return new(statement, EOFToken);
        }

        //If it doesn't fit any of our current conditions, we take it as an expression
        private StatementSyntax ParseStatement()
            => Current.Kind switch
            {
                SyntaxKind.OpenCurlyBrace => ParseBlockStatement(),
                SyntaxKind.LetConstKeyword or 
                SyntaxKind.ConstKeyword or 
                SyntaxKind.VarKeyword => ParseVariableDeclaration(),
                // _ => is just fancy for default
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
            var expected = Current.Kind;
            var keyword = MatchToken(expected);
            var identifier = MatchToken(SyntaxKind.IdentifierToken);
            var equals = MatchToken(SyntaxKind.Equals);
            var initializer = ParseExpression();
            return new VariableDeclarationSyntax(keyword, identifier, equals, initializer);
        }

        //This just calls parse expression
        private ExpressionStatementSyntax ParseExpressionStatement() => new(ParseExpression());

        //This is just another wrapper function to go through our heirarchy
        private ExpressionSyntax ParseExpression()
        {
            return ParseAssignmentExpression();
        }

        //ParseExpression calls this
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
            //Declaring left so that we can use it in the entire method
            ExpressionSyntax left;

            //Checking to see if it's a unary operator
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
                //If UnareOperatorPrecedence is greater than or equal to ParentPrecedence, we don't get here, if it isn't
                //we then walk the left side of our tree again
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
            //Console.WriteLine(left);
            return left;
        }


        private ExpressionSyntax ParsePrimaryExpression()
            //Then we go here
            =>  Current.Kind switch
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
        //This leaves us with a split road, we can parse either a:
        //  Number literal (ie 1234, 512, 12039)
        //  A parenthesized expression (ie 1 + (2 * 3))
        //  A bool (true/false)
        //  Or by default we assume it's a name expression.

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
            //Console.WriteLine(Current);
            var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
            return new NameExpressionSyntax(identifierToken);
        }

    }
}
