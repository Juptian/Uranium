﻿using System;
using System.Collections.Generic;
using Uranium.CodeAnalysis.Binding;
using Uranium.CodeAnalysis.Binding.Statements;
using Uranium.CodeAnalysis.Binding.NodeKinds;
using Uranium.CodeAnalysis.Text;
using Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Types;
using Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Expression;

namespace Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Statements
{
    internal static class IfStatementEvaluator
    {
        public static void EvaluateIfStatement(BoundIfStatement statement, Evaluator eval)
        {
            if ( (bool)ExpressionEvaluator.EvaluateExpression(statement.Condition, eval) )
            {
                eval.EvaluateStatement(statement.Statement);
            }
            else if (statement.ElseStatement is not null)
            {
                eval.EvaluateStatement(statement.ElseStatement);
            }
        }
    }
}
