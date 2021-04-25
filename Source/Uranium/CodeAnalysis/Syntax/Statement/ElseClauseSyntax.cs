using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Syntax.Statement
{
    public sealed class ElseClauseSyntax : SyntaxNode
    {
        public ElseClauseSyntax(SyntaxToken elseKeyword, StatementSyntax elseStatement)
        {
            ElseKeyword = elseKeyword;
            ElseStatement = elseStatement;
        }

        public SyntaxToken ElseKeyword { get; }
        public StatementSyntax ElseStatement { get; }

        public override SyntaxKind Kind => SyntaxKind.ElseStatement;
    }
}
