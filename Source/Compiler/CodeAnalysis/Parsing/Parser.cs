using System;
using Compiler.CodeAnalysis.Lexing;
using Compiler.CodeAnalysis.Syntax.Expression;
using System.Collections.Generic;
using System.Linq;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Parsing
{
    internal sealed class Parser
    {
        private readonly SyntaxToken[] _tokens;
        private int _position;
        private SyntaxToken _current => Peek(0);
        private readonly List<string> _diagnostics = new();

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
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

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
            var current = _current;
            _position++;
            return current;
        }

        private SyntaxToken MatchToken(SyntaxKind kind)
        {
            if (_current.Kind == kind)
            {
                return NextToken();
            }

            _diagnostics.Add($"Unexpected token: {_current.Kind}, expected {kind}");

            return new(kind, _current.Position, null, null);
        }

        public SyntaxTree Parse()
        {
            var expression = ParseExpression();
            var EOFToken = MatchToken(SyntaxKind.EndOfFile);

            return new(_diagnostics, expression, EOFToken);
        }

        //Allowing for proper operator precedence
        private ExpressionSyntax ParseExpression(int parentPrecedence = 0)
        {
            var left = ParsePrimaryExpression();
            
            //Keep looping until our precedence is <= parent precedence, or == 0;
            while(true)
            {
                var precedence = GetBinaryOperatorPrecedence(_current.Kind);
                if(precedence == 0 || precedence <= parentPrecedence)
                {
                    break;
                }

                var operatorToken = NextToken();
                var right = ParseExpression(precedence);

                left = new BinaryExpressionSyntax(left, operatorToken, right);

            }
            return left;
        }

        private static int GetBinaryOperatorPrecedence(SyntaxKind kind)
        {
            switch(kind)
            {
                //Just operator precedence
                case SyntaxKind.Plus:
                case SyntaxKind.Minus:
                    return 1;

                case SyntaxKind.Multiply:
                case SyntaxKind.Divide:
                    return 2;

                case SyntaxKind.Pow:
                    return 3;

                default:
                    return 0;
            }
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            if (_current.Kind == SyntaxKind.OpenParenthesis)
            {
                var left = NextToken();
                var expression = ParseExpression();
                var right = MatchToken(SyntaxKind.CloseParenthesis);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            var numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }

        public static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            var marker = isLast ? "└───" : "├───";

            Console.Write(indent + marker + node.Kind);

            if (node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" " + t.Value);
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
