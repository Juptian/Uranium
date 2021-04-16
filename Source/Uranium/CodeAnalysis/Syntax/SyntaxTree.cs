using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Syntax.Expression;
using Uranium.CodeAnalysis.Parsing;
using Uranium.Logging;
using Uranium.CodeAnalysis.Lexing;
using Uranium.CodeAnalysis.Text;

namespace Uranium.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(SourceText text, ImmutableArray<Diagnostic> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
        {
            Text = text;
            Diagnostics = diagnostics;
            Root = root;
            EndOfFileToken = endOfFileToken;
        }

        public SourceText Text { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }

        public static SyntaxTree Parse(string text)
        {
            var sourceText = SourceText.From(text);
            return Parse(sourceText);
        }

        public static SyntaxTree Parse(SourceText text)
        {
            var parser = new Parser(text);
            return parser.Parse();
        }


        public static IEnumerable<SyntaxToken> LexTokens(string text)
        {
            var sourceText = SourceText.From(text);
            return LexTokens(sourceText);
        }

        public static IEnumerable<SyntaxToken> LexTokens(SourceText text)
        {
            //Console.WriteLine(text);
            var lexer = new Lexer(text);
            while(true)
            {
                var token = lexer.Lex();
                if (token.Kind == SyntaxKind.EndOfFile)
                {
                    break;
                }
                yield return token;
            }
        }

    }
}
