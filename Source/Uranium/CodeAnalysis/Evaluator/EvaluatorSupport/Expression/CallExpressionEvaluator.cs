using System;
using Uranium.CodeAnalysis.Binding.NodeKinds;
using Uranium.CodeAnalysis.Symbols;

namespace Uranium.CodeAnalysis.Syntax.EvaluatorSupport
{
    internal static class CallExpressionEvaluator
    {
        internal static object? Evaluate(BoundCallExpression statement, Evaluator evaluator)
        {
            if(statement.Function == BuiltInFunctions.Println)
            {
                var message = ExpressionEvaluator.Evaluate(statement.Arguments[0], evaluator)!.ToString();
                Console.WriteLine(message);
            }
            else if(statement.Function == BuiltInFunctions.Print)
            {
                var message = ExpressionEvaluator.Evaluate(statement.Arguments[0], evaluator)!.ToString();
                Console.Write(message);
            }
            else if(statement.Function == BuiltInFunctions.Input)
            {
                return Console.ReadLine();
            }
            else
            {
                throw new Exception($"Unexpected function ${statement.Function.Name}");
            }
            return null;
        }
    }
}