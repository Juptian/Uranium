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
    internal static class ForStatementEvaluator
    {
        public static void EvaluateForStatement(BoundForStatement statement, Evaluator eval)
        {
            if (statement.VariableDeclaration is not null)
            {
                eval.EvaluateStatement(statement.VariableDeclaration);
            }
            while (statement.Condition is null || (bool)ExpressionEvaluator.EvaluateExpression(statement.Condition, eval))
            {
                eval.EvaluateStatement(statement.Body);
                if (statement.Increment is not null)
                {
                    ExpressionEvaluator.EvaluateExpression(statement.Increment, eval);
                }
            }
        }


    }
}
