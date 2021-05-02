using System;
using System.Collections.Generic;
using Uranium.CodeAnalysis.Lexing;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Expression;
using Uranium.CodeAnalysis.Syntax.Statement;
using Uranium.CodeAnalysis.Text;
using Uranium.Logging;
using Uranium.CodeAnalysis.Parsing.ParserSupport;

namespace Uranium.CodeAnalysis.Parsing
{
    internal sealed class Parser
    {
        internal readonly SyntaxToken[] _tokens;
        internal int Position;
        internal readonly DiagnosticBag _diagnostics = new();

        public Parser(SourceText text)
        {
            var tokens = new List<SyntaxToken>();
            var lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.Lex();
                if (!IsIgnoredToken(token))
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

        internal SyntaxToken Current => Peek(0);
        internal SyntaxToken Next => Peek(1);
        public DiagnosticBag Diagnostics => _diagnostics;
        
        internal SyntaxToken Peek(int offset)
        {
            var index = Position + offset;
            if (index >= _tokens.Length)
            {
                return _tokens[^1];
            }
            return _tokens[index];
        }

        internal SyntaxToken NextToken()
        {
            Position++;
            return Peek(-1);
        }

        internal SyntaxToken MatchToken(SyntaxKind kind)
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
            var statement = StatementParser.Parse(this);
            var EndOfFileToken = MatchToken(SyntaxKind.EndOfFile);
            return new(statement, EndOfFileToken);
        }
        
        //Two different return types here so two different functions
        internal static ExpressionStatementSyntax ParseExpressionStatement(Parser parser) => new(ParseExpression(parser));
        internal static ExpressionSyntax ParseExpression(Parser parser) => AssignmentExpressionParser.Parse(parser);

        internal static bool IsIgnoredToken(SyntaxToken token)
            => token.Kind is SyntaxKind.WhiteSpace || 
               token.Kind is SyntaxKind.BadToken ||
               token.Kind is SyntaxKind.LineBreak;
    }
}
