using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Syntax.Expression;

namespace Uranium.CodeAnalysis.Syntax.Statement
{
    public sealed class IfStatementSyntax : StatementSyntax
    {
        public IfStatementSyntax
            (
            SyntaxToken ifKeyword, 
            SyntaxToken openParenthesis, ExpressionSyntax condition, SyntaxToken closeParenthesis,
            BlockStatementSyntax body, 
            ElseClauseSyntax? elseClause
            )
        {
            IfKeyword = ifKeyword;
            OpenParenthesis = openParenthesis;
            Condition = condition;
            CloseParenthesis = closeParenthesis;
            Body = body;
            ElseClause = elseClause;
        }

        public SyntaxToken IfKeyword { get; }
        public SyntaxToken OpenParenthesis { get; }
        public ExpressionSyntax Condition { get; }
        public SyntaxToken CloseParenthesis { get; }
        public BlockStatementSyntax Body { get; }
        public ElseClauseSyntax? ElseClause { get; }

        public override SyntaxKind Kind => SyntaxKind.IfStatement;
    }
}
