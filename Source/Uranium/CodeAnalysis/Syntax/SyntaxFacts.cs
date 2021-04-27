using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Syntax.SyntaxFactsSupport;

namespace Uranium.CodeAnalysis.Syntax
{
    public static class SyntaxFacts
    {
        //Binary operators
        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
            => SyntaxFactsOperators.GetBinaryOperatorPrecedence(kind);
        
        public static IEnumerable<SyntaxKind> GetBinaryOperators()
            => SyntaxFactsOperators.GetBinaryOperators();

        //Unary operators
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
            => SyntaxFactsOperators.GetUnaryOperatorPrecedence(kind);
        //Bound to 4 so that our tree looks correct, result is the same regardless
        //-1 * 3
        //
        //
        //    *
        //   / \
        //  -   3
        //  |
        //  1
        //Because this is how math works!

        public static IEnumerable<SyntaxKind> GetUnaryOperators()
            => SyntaxFactsOperators.GetUnaryOperators();

        public static bool CheckForCompoundOperator(SyntaxToken token)
            => SyntaxFactsOperators.CheckForCompoundOperator(token);

        public static SyntaxToken GetSoloOperator(SyntaxToken token)
            => SyntaxFactsOperators.GetSoloOperator(token);

        internal static SyntaxKind GetKeywordKind(string text)
            => SyntaxFactsText.GetKeywordKind(text);

        public static string GetText(SyntaxKind kind) 
            => SyntaxFactsText.GetText(kind);

        public static Type? GetKeywordType(SyntaxKind kind)
            => SyntaxFactsText.GetKeywordType(kind);

        public static Type? GetKeywordType(string kind)
            => SyntaxFactsText.GetKeywordType(kind); 

    }
}
