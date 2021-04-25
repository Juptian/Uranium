using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Syntax.Expression;

namespace Uranium.CodeAnalysis.Syntax.Statement
{
    public sealed class WhileStatementSyntax : StatementSyntax
    {
        public WhileStatementSyntax(SyntaxToken whileKeyword, ExpressionSyntax expression, StatementSyntax body)
        {
            WhileKeyword = whileKeyword;
            Expression = expression;
            Body = body;
        }

        public SyntaxToken WhileKeyword { get; }
        public ExpressionSyntax Expression { get; }
        public StatementSyntax Body { get; }

        public override SyntaxKind Kind => SyntaxKind.WhileStatement;
    }
}
