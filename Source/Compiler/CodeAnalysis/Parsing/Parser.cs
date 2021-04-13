using System;
using Compiler.CodeAnalysis.Lexing;
using Compiler.CodeAnalysis.Syntax.Expression;
using System.Collections.Generic;
using System.Linq;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Parsing
{
    internal class Parser
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
                token = lexer.NextToken();
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

        private SyntaxToken Match(SyntaxKind kind)
        {
            if (_current.Kind == kind)
            {
                return NextToken();
            }

            _diagnostics.Add($"Unexpected token: {_current.Kind}, expected {kind}");

            return new(kind, _current.Position, null, null);
        }

        private ExpressionSyntax ParseExpression()
        {
            return ParseTerm();
        }

        public SyntaxTree Parse()
        {
            var expression = ParseTerm();
            var EOFToken = Match(SyntaxKind.EndOfFile);

            return new(_diagnostics, expression, EOFToken);
        }

        private ExpressionSyntax ParseTerm()
        {
            var left = ParseFactor();

            while (_current.Kind == SyntaxKind.Plus || _current.Kind == SyntaxKind.Minus)
            {
                if (_position >= _tokens.Length)
                {
                    break;
                }
                var operatorToken = NextToken();
                var right = ParseFactor();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParseFactor()
        {
            var left = ParsePrimaryExpression();

            while (_current.Kind == SyntaxKind.Divide || _current.Kind == SyntaxKind.Pow || _current.Kind == SyntaxKind.Multiply)
            {
                if (_position >= _tokens.Length)
                {
                    break;
                }
                var operatorToken = NextToken();
                var right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;

        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            if (_current.Kind == SyntaxKind.OpenParenthesis)
            {
                var left = NextToken();
                var expression = ParseExpression();
                var right = Match(SyntaxKind.CloseParenthesis);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            var numberToken = Match(SyntaxKind.NumberToken);
            return new NumberExpressionSyntax(numberToken);
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
