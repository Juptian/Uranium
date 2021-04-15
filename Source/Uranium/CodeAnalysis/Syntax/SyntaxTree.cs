using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Syntax.Expression;
using Uranium.CodeAnalysis.Parsing;
using Uranium.Logging;
using Uranium.CodeAnalysis.Lexing;

namespace Uranium.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(IEnumerable<Diagnostic> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
            EndOfFileToken = endOfFileToken;
        }

        public IReadOnlyList<Diagnostic> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }

        public static SyntaxTree Parse(string text)
        {
            var parser = new Parser(text);
            return parser.Parse();
        }

        public static IEnumerable<SyntaxToken> LexTokens(string text)
        {
            Console.WriteLine(text);
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
