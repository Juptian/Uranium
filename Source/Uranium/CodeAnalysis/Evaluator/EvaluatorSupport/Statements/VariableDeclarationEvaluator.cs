using System;
using Uranium.CodeAnalysis.Binding.NodeKinds;
using Uranium.CodeAnalysis.Binding.Statements;
using Uranium.CodeAnalysis.Symbols;

namespace Uranium.CodeAnalysis.Syntax.EvaluatorSupport
{
    internal static class VariableDeclarationEvaluator
    {
        public static void Evaluate(BoundVariableDeclaration statement, Evaluator eval)
        {
            object? value = null;
            if(statement.Initializer is not null)
            {
                value = ExpressionEvaluator.Evaluate(statement.Initializer, eval);
                if(value is not null)
                {
                    if(value!.GetType() != TypeChecker.GetType(statement.Variable.Type))
                    {
                        var conversion = CreateConversionExpression(value, statement.Variable.Type);
                        var newDeclaration = new BoundVariableDeclaration(statement.Variable, conversion);
                        Evaluate(newDeclaration, eval);
                        return;
                    }
                }
            }
            eval.Variables[statement.Variable] = value;
            eval.LastValue = value;
        }

        private static BoundConversionExpression CreateConversionExpression(object value, TypeSymbol type)
        {
            var literalExpression = new BoundLiteralExpression(value);
            return new BoundConversionExpression(type, literalExpression);
        }
    }
}
