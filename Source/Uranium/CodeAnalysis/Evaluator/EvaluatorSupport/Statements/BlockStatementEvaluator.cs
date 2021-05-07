﻿using System;
using Uranium.CodeAnalysis.Binding.Statements;

namespace Uranium.CodeAnalysis.Syntax.EvaluatorSupport
{
    internal static class BlockStatementEvaluator
    {
        public static void Evaluate(BoundBlockStatement statement, Evaluator eval)
        {
            foreach (var item in statement.Statements)
            {
                eval.EvaluateStatement(item);
            }
        }
    }
}