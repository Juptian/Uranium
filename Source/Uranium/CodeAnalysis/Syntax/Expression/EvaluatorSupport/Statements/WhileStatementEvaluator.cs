using System;
using System.Collections.Generic;
using Uranium.CodeAnalysis.Binding;
using Uranium.CodeAnalysis.Binding.Statements;
using Uranium.CodeAnalysis.Binding.NodeKinds;
using Uranium.CodeAnalysis.Text;
using Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Types;
using Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Expression;


namespace Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Statements
{
    internal static class WhileStatementEvaluator
    {
        public static void EvaluateWhileStatement(BoundWhileStatement statement, Evaluator eval)
        {
            while ( (bool)ExpressionEvaluator.EvaluateExpression(statement.Condition, eval))
            {
                eval.EvaluateStatement(statement.Body);
            }
        }
    }
}
