using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Expression
{
    internal static class EqualityEvaluator
    {

        public static bool LeftEqualsRight(object left, object right)
        {
            if (right is bool rightAsBool && TryGetNumber(left, out var leftAsDouble))
                return rightAsBool == (leftAsDouble != 0.0);

            if (left is bool leftAsBool && TryGetNumber(right, out var rightAsDouble))
                return leftAsBool == (rightAsDouble != 0.0);

            return Equals(left, right);
        }

        private static bool TryGetNumber(object value, out double result)
        {
            result = 0.0;

            var isNumber = IsNumber(value.GetType());
            if (isNumber)
                result = Convert.ToDouble(value);

            return isNumber;
        }

        private static bool IsNumber(Type type) => IsFloatingPointNumber(type) || IsInteger(type);
        private static bool IsFloatingPointNumber(Type type) => type == typeof(float) || type == typeof(double);
        private static bool IsInteger(Type type) => type == typeof(int) || type == typeof(long);
    }
}
