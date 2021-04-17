using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Syntax.Expression;
using Uranium.CodeAnalysis.Syntax.Statement;

namespace Uranium.CodeAnalysis.Syntax
{
    public sealed class CompilationUnitSyntax : SyntaxNode
    {
        public CompilationUnitSyntax(StatementSyntax statement, SyntaxToken endOfFileToken)
        {
            Statement = statement;
            EndOfFileToken = endOfFileToken;
        }

        public StatementSyntax Statement { get; }
        public SyntaxToken EndOfFileToken { get; }

        public override SyntaxKind Kind => SyntaxKind.CompilationUnit;
    }
}
