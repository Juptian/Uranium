using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport
{
    internal static class EvaluatorEquals
    {

        public static bool LeftEqualsRight(object left, object right)
        {
            var leftIsNumber = IsNumber(left.GetType()); 

            var rightIsNumber = IsNumber(right.GetType());

            if (leftIsNumber && right.GetType() == typeof(bool))
            {
                if(IsFloatingPointNumber(left.GetType()))
                {
                    return FloatingPointNumbersLeft(left, right); 
                }
                return IntegerNumbersLeft(left, right);
            }
            else if (rightIsNumber && left.GetType() == typeof(bool))
            {
                if(IsFloatingPointNumber(right.GetType()))
                {
                    return FloatingPointNumbersRight(left, right);
                }
                return IntegerNumbersRight(left, right);
            }
            return Equals(left, right);
        }

        public static bool IsFloatingPointNumber(Type type)
            => type == typeof(float) || type == typeof(double);

        public static bool IsInteger(Type type)
            => type == typeof(int) || type == typeof(long);

        private static bool IsNumber(Type type)
            => IsFloatingPointNumber(type) || IsInteger(type);

        private static bool FloatingPointNumbersLeft(object left, object right)
        {
            if(left.GetType() == typeof(float))
            {
                return (float)left == 0 ? !(bool)right : (bool)right;
            }
            return (double)left == 0 ? !(bool)right : (bool)right;
        }

        private static bool FloatingPointNumbersRight(object left, object right)
            => FloatingPointNumbersLeft(right, left);

        private static bool IntegerNumbersLeft(object left, object right)
        {
            if(left.GetType() == typeof(int))
            {
                return (int)left == 0 ? !(bool)right : (bool)right;
            }
            return (long)left == 0 ? !(bool)right : (bool)right;
        }

        private static bool IntegerNumbersRight(object left, object right)
            => IntegerNumbersLeft(right, left);


    }
}
