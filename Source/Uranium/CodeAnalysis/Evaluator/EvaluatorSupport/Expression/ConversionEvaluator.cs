using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Binding.NodeKinds;
using Uranium.CodeAnalysis.Symbols;

namespace Uranium.CodeAnalysis.Syntax.EvaluatorSupport
{
    internal static class ConversionEvaluator
    {
        public static object Evaluate(BoundConversionExpression node, Evaluator eval)
        {
            var value = ExpressionEvaluator.Evaluate(node.Expression, eval);
            object? throwaway = false;
            ConvertBoolToInt(ref value, ref throwaway);
            if(node.Type == TypeSymbol.Bool)
            {
                ConvertToBool(value);
            }
            else if(node.Type == TypeSymbol.Int)
            {
                return Convert.ToInt32(value);
            }
            else if(node.Type == TypeSymbol.Long)
            {
                return Convert.ToInt64(value);
            }
            else if(node.Type == TypeSymbol.Float)
            {
                return Convert.ToSingle(value);
            }
            else if(node.Type == TypeSymbol.Double)
            {
                return Convert.ToDouble(value);
            }
            else if(node.Type == TypeSymbol.String)
            {
                return Convert.ToString(value) ?? string.Empty;
            }
            throw new Exception($"Unexpected type {node.Type}");
        }

        public static bool ConvertToBool(object? obj)
        {
            return obj switch
            {
                int => (int)obj != 0,
                long => (long)obj != 0,
                float => (float)obj != 0,
                double => (double)obj != 0,
                string => (string)obj == "true" || (string)obj != "0",
                char => (char)obj != '0',
                _ => Convert.ToBoolean(obj)
            };
        }

        public static void ConvertBoolToInt(ref object? left, ref object? right)
        {
            if(left is null) { left = 0; }
            if(right is null) { right = 0; }

            int newRight = -1;
            int newLeft = -1;
            if (right is bool r)
            {
                newRight = r ? 1 : 0;
            }
            if(left is bool l)
            {
                newLeft = l ? 1 : 0;
            }
            left = newLeft < 0 ? left : newLeft;
            right = newRight < 0 ? right : newRight;
        }
    }
}
