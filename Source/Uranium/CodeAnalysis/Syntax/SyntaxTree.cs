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
        private SyntaxTree(SourceText text)
        {
            var parser = new Parser(text);
            var root = parser.ParseCompilationUnit();

            Text = text;
            //After all that, we just add our diagnostics that got reported
            Diagnostics = parser.Diagnostics.ToImmutableArray();
            Root = root;
        }

        public SourceText Text { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public CompilationUnitSyntax Root { get; }

        public static SyntaxTree Parse(string text)
        {
            var sourceText = SourceText.From(text);
            return Parse(sourceText);
        }
    
        public static SyntaxTree Parse(SourceText text)
        {
            return new(text);
        }

        public static IEnumerable<SyntaxToken> LexTokens(string text)
        {
            var sourceText = SourceText.From(text);
            return LexTokens(sourceText);
        }

        public static IEnumerable<SyntaxToken> LexTokens(SourceText text)
        {
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
