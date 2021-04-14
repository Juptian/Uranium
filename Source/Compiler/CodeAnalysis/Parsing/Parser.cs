using System;
using Compiler.CodeAnalysis.Lexing;
using Compiler.CodeAnalysis.Syntax.Expression;
using System.Collections.Generic;
using System.Linq;
using Compiler.CodeAnalysis.Syntax;
using Compiler.Logging;

namespace Compiler.CodeAnalysis.Parsing
{
    internal sealed class Parser
    {
        private readonly SyntaxToken[] _tokens;
        private int _position;
        private readonly DiagnosticBag _diagnostics = new();

        public Parser(string text)
        {
            var tokens = new List<SyntaxToken>();
            var lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.Lex();
                if (token.Kind != SyntaxKind.WhiteSpace || token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            }
            while (token.Kind != SyntaxKind.EndOfFile);
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

            return new(kind, Current.Position, null, null);
        }

        public SyntaxTree Parse()
        {
            var expression = ParseExpression();
            var EOFToken = MatchToken(SyntaxKind.EndOfFile);

            return new(_diagnostics, expression, EOFToken);
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
            switch(Current.Kind)
            {
                //Parenthesis
                case SyntaxKind.OpenParenthesis:
                    var left = NextToken();
                    var expression = ParseExpression();
                    var right = MatchToken(SyntaxKind.CloseParenthesis);
                    return new ParenthesizedExpressionSyntax(left, expression, right);
                
                //Bools
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    var keywordToken = NextToken();
                    var value = keywordToken.Kind == SyntaxKind.TrueKeyword;
                    return new LiteralExpressionSyntax(keywordToken, value);

                //Identifiers
                case SyntaxKind.IdentifierToken:
                    var identifierToken = NextToken();

                    return new NameExpressionSyntax(identifierToken);

                //Assuming default is a number token
                default:
                    var numberToken = MatchToken(SyntaxKind.NumberToken);
                    return new LiteralExpressionSyntax(numberToken);

            }       
        }

        public static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            var marker = isLast ? "└───" : "├───";

            Console.Write(indent + marker + node.Kind);

            if (node is SyntaxToken token && token.Value is not null)
            {
                Console.Write(" " + token.Value);
            }
            Console.WriteLine();
            indent += isLast ? "    " : "│   ";

            var lastChild = node.GetChildren().LastOrDefault();
            foreach (var child in node.GetChildren())
            {
                PrettyPrint(child, indent, child == lastChild);
            }
        }
    }
}
