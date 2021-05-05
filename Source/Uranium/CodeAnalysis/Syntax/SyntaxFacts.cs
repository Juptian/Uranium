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
            => OperatorChecker.GetBinaryOperatorPrecedence(kind);
        
        public static IEnumerable<SyntaxKind> GetBinaryOperators()
            => OperatorChecker.GetBinaryOperators();

        //Unary operators
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
            => OperatorChecker.GetUnaryOperatorPrecedence(kind);
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
            => OperatorChecker.GetUnaryOperators();

        public static bool CheckForCompoundOperator(SyntaxToken token)
            => OperatorChecker.CheckForCompoundOperator(token);

        public static SyntaxToken GetSoloOperator(SyntaxToken token)
            => OperatorChecker.GetSoloOperator(token);

        public static SyntaxKind GetSoloOperator(SyntaxKind kind)
            => OperatorChecker.GetSoloOperator(kind);

        public static SyntaxKind GetKind(string text)
            => TextChecker.GetSyntaxKind(text);

        public static string GetText(SyntaxKind kind) 
            => TextChecker.GetText(kind);

        public static Type? GetKeywordType(SyntaxKind kind)
            => TextChecker.GetKeywordType(kind);

        public static Type? GetKeywordType(string kind)
            => TextChecker.GetKeywordType(kind);

        public static SyntaxKind GetKeyword(object obj)
            => TextChecker.GetKeyword(obj);

        public static bool IsVarKeyword(SyntaxKind kind)
            => TextChecker.IsVarKeyword(kind);

        public static bool IsNumber(object obj)
            => IsFloatingPoint(TextChecker.GetKeyword(obj)) || IsInteger(TextChecker.GetKeyword(obj));

        public static bool IsFloatingPoint(SyntaxKind kind)
            => TypeChecker.IsFloatingPoint(kind);

        public static bool IsInteger(SyntaxKind kind)
            => TypeChecker.IsInteger(kind);

    }
}
