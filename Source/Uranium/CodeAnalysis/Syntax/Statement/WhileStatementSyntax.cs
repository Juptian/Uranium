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
        public WhileStatementSyntax(SyntaxToken whileKeyword, SyntaxToken openParenthesis, ExpressionSyntax expression, SyntaxToken closeParenthesis, BlockStatementSyntax body)
        {
            WhileKeyword = whileKeyword;
            OpenParenthesis = openParenthesis;
            Expression = expression;
            CloseParenthesis = closeParenthesis;
            Body = body;
        }

        public SyntaxToken WhileKeyword { get; }
        public SyntaxToken OpenParenthesis { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParenthesis { get; }
        public BlockStatementSyntax Body { get; }

        public override SyntaxKind Kind => SyntaxKind.WhileStatement;
    }
}
