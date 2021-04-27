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
    internal static class BlockStatementEvaluator
    {
        public static void EvaluateBlockStatement(BoundBlockStatement statement, Evaluator eval)
        {
            foreach (var item in statement.Statements)
            {
                eval.EvaluateStatement(item);
            }
        }


    }
}
