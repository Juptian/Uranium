using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Syntax.Expression;

namespace Uranium.CodeAnalysis.Syntax.Statement
{
    public sealed class ForStatementSyntax : StatementSyntax
    {
        //for var a = 10; a < 11; a = a + 1
        public ForStatementSyntax(
            SyntaxToken forKeyword, SyntaxToken openParenthesisToken,
            StatementSyntax? variable, SyntaxToken initializeSemicolon,
            ExpressionSyntax? condition, SyntaxToken conditionSemicolon,
            ExpressionSyntax? incrementation, SyntaxToken closeParenthesisToken,
            BlockStatementSyntax body
            )
        {
            ForKeyword = forKeyword;
            OpenParenthesisToken = openParenthesisToken;
            Variable = variable;
            InitializeSemicolon = initializeSemicolon;
            Condition = condition;
            ConditionSemicolon = conditionSemicolon;
            Incrementation = incrementation;
            CloseParenthesisToken = closeParenthesisToken;
            Body = body;
        }

        public SyntaxToken ForKeyword { get; }
        public SyntaxToken OpenParenthesisToken { get; }
        public StatementSyntax? Variable { get; }
        public SyntaxToken InitializeSemicolon { get; }
        public ExpressionSyntax? Condition { get; }
        public SyntaxToken ConditionSemicolon { get; }
        public ExpressionSyntax? Incrementation { get; }
        public SyntaxToken CloseParenthesisToken { get; }
        public BlockStatementSyntax Body { get; }

        public override SyntaxKind Kind => SyntaxKind.ForStatement;
    }
}
