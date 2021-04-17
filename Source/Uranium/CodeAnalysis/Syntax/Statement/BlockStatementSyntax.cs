using System;
using System.Collections.Generic;
using System.Collections.Immutable; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Syntax.Statement
{
    public sealed class BlockStatementSyntax : StatementSyntax
    {
        public BlockStatementSyntax(SyntaxToken openBraceToken, ImmutableArray<StatementSyntax> statements, SyntaxToken closingBraceToken)
        {
            OpenBraceToken = openBraceToken;
            Statements = statements;
            ClosingBraceToken = closingBraceToken;
        }

        public SyntaxToken OpenBraceToken { get; }
        public ImmutableArray<StatementSyntax> Statements { get; }
        public SyntaxToken ClosingBraceToken { get; }

        public override SyntaxKind Kind => SyntaxKind.BlockStatement;
    }
}
