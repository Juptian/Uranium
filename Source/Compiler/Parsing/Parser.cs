using System;
using Compiler.Lexing;
using Compiler.Syntax;
using Compiler.Syntax.Expression;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.Parsing
{
    class Parser
    {
        private readonly SyntaxToken[] _tokens;
        private int _position = 0;
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

        private SyntaxToken Current => Peek(0);
        public IEnumerable<string> Diagnostics => _diagnostics;


        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _tokens.Length)
                return _tokens[^1];
            return _tokens[index];
        }

        private SyntaxToken NextToken() 
        {
            var current = Current;
            _position++;
            return current;
        }
            
        private SyntaxToken Match(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();

            //_diagnostics.Add($"Unexpected token: {Current.Kind}, expected {kind}");

            return new SyntaxToken(kind, Current.Position, null, null);
        }

        private ExpressionSyntax ParseExpression()
        {
            return ParseTerm();
        }

        public SyntaxTree Parse()
        {
            var expression = ParseTerm();
            var EOFToken = Match(SyntaxKind.EndOfFile);

            return new SyntaxTree(_diagnostics, expression, EOFToken);
        }

        private ExpressionSyntax ParseTerm()
        {
            var left = ParsePrimaryExpression();

            while(Current.Kind == SyntaxKind.Plus || Current.Kind == SyntaxKind.Minus)
            {
                if (_position >= _tokens.Length) break;
                var operatorToken = NextToken();
                var right = ParseFactor();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;

        }

        private ExpressionSyntax ParseFactor()
        {
            var left = ParsePrimaryExpression();

            while( Current.Kind == SyntaxKind.Divide || Current.Kind == SyntaxKind.Pow || Current.Kind == SyntaxKind.Multiply)
            {
                if (_position >= _tokens.Length) break;
                var operatorToken = NextToken();
                var right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;

        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            if(Current.Kind == SyntaxKind.OpenParenthesis)
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
            
            if(node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" " + t.Value);
            }
            Console.WriteLine();
            indent += isLast ? "    " : "│   ";

            var lastChild = node.GetChildren().LastOrDefault();
            foreach (var child in node.GetChildren())
                PrettyPrint(child, indent, child == lastChild);
        }
    }
}
