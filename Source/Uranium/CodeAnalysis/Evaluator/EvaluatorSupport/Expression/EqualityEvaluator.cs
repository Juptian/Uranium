using System;

namespace Uranium.CodeAnalysis.Syntax.EvaluatorSupport
{
    internal static class EqualityEvaluator
    {

        public static bool LeftEqualsRight(object? left, object? right)
        {
            if(left is null) { throw new ArgumentNullException("Left hand of operand may not be null"); }
            if(right is null) { throw new ArgumentNullException("Right hand of operand may not be null"); }
            if (right is bool rightAsBool && TryGetNumber(left, out var leftAsDouble))
                return rightAsBool == (leftAsDouble != 0.0);

            if (left is bool leftAsBool && TryGetNumber(right, out var rightAsDouble))
                return leftAsBool == (rightAsDouble != 0.0);

            return Equals(left, right);
        }

        private static bool TryGetNumber(object value, out double result)
        {
            result = 0.0;

            var isNumber = TypeChecker.IsNumber(value);
            if (isNumber)
                result = Convert.ToDouble(value);

            return isNumber;
        }

 
    }
}
