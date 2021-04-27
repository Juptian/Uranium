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
    internal static class VariableDeclarationEvaluator
    {
        public static void EvaluateVariableDeclaration(BoundVariableDeclaration statement, Evaluator eval)
        {
            var value = ExpressionEvaluator.EvaluateExpression(statement.Initializer, eval);
            eval.Variables[statement.Variable] = value;
            eval.LastValue = value;
        }

    }
}
