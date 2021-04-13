using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Compiler.Lexing;

namespace Compiler.Syntax.Expression
{
    sealed class NumberExpressionSyntax : ExpressionSyntax
    {
        public NumberExpressionSyntax(SyntaxToken numberToken)
        {
            NumberToken = numberToken;
        }
        public override SyntaxKind Kind => SyntaxKind.NumberExpression;
        public SyntaxToken NumberToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NumberToken; 
        }
    }
}
