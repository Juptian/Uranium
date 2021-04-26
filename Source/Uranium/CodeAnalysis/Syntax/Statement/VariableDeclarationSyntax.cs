using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Syntax.Expression;

namespace Uranium.CodeAnalysis.Syntax.Statement
{
    public sealed class VariableDeclarationSyntax : StatementSyntax
    {
        public VariableDeclarationSyntax
            (
            SyntaxToken keywordToken,
            SyntaxToken identifier, 
            SyntaxToken equalsToken, 
            ExpressionSyntax initializer,
            SyntaxToken semicolon
            )
        {
            KeywordToken = keywordToken;
            Identifier = identifier;
            EqualsToken = equalsToken;
            Initializer = initializer;
            Semicolon = semicolon;
        }

        public override SyntaxKind Kind => SyntaxKind.VariableDeclaration;

        public SyntaxToken KeywordToken { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax Initializer { get; }
        public SyntaxToken Semicolon { get; }
    }
}
