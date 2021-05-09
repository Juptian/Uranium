using System;
using System.Numerics;

#pragma warning disable IDE0038 // Use pattern matching
#pragma warning disable IDE0066 // Convert switch statement to expressio
// Every time I've tried I've failed for an unkown reason
namespace Uranium.CodeAnalysis.Syntax.EvaluatorSupport
{
    internal static class Operations
    {
        private const int intValue = 1;
        private const int floatValue = 2;
        private const int longValue = 3;
        private const int doubleValue = 4;
        public static object Addition(object left, object right)
        {
            ConvertBoolToInt(ref left, ref right);

            var leftPrio = SyntaxFacts.GetTypePriority(left);
            var rightPrio = SyntaxFacts.GetTypePriority(right);
            
            switch(leftPrio)
            {
                case intValue when rightPrio == leftPrio:
                    return (int)left + (int)right;
                case floatValue when rightPrio == leftPrio:
                    return (float)left + (float)right;
                case longValue when rightPrio == leftPrio:
                    return (long)left + (long)right;
                case doubleValue when rightPrio == leftPrio:
                    return (double)left + (double)right;

                case intValue when rightPrio == floatValue:
                    return (int)((int)left + (float)right);
                case intValue when rightPrio == longValue:
                    return (int)((int)left + (long)right);
                case intValue when rightPrio == doubleValue:
                    return (int)((int)left + (double)right);

                case floatValue when rightPrio == intValue:
                    return (float)((float)left + (int)right);
                case floatValue when rightPrio == doubleValue:
                    return (float)((float)left + (double)right);

                case longValue when rightPrio == intValue:
                    return (long)left + (int)right;
                
                case doubleValue when rightPrio == floatValue:
                    return (double)((double)left + (float)right);
                case doubleValue when rightPrio == intValue:
                    return (double)((double)left + (int)right);
                default:
                    throw new($"Invalid type {left.GetType().ToString()[7..]} for binary operand with {right.GetType().ToString()[7..]}");
            }
            
        }
        public static object Subtraction(object left, object right)
        {
            ConvertBoolToInt(ref left, ref right);

            var leftPrio = SyntaxFacts.GetTypePriority(left);
            var rightPrio = SyntaxFacts.GetTypePriority(right);
            switch(leftPrio)
            {
                case intValue when rightPrio == leftPrio:
                    return (int)left - (int)right;
                case floatValue when rightPrio == leftPrio:
                    return (float)left - (float)right;
                case longValue when rightPrio == leftPrio:
                    return (long)left - (long)right;
                case doubleValue when rightPrio == leftPrio:
                    return (double)left - (double)right;

                case intValue when rightPrio == floatValue:
                    return (int)((int)left - (float)right);
                case intValue when rightPrio == longValue:
                    return (int)((int)left - (long)right);

                case intValue when rightPrio == doubleValue:
                    return (int)((int)left - (double)right);

                case floatValue when rightPrio == intValue:
                    return (float)((float)left - (int)right);
                case floatValue when rightPrio == doubleValue:
                    return (float)((float)left - (double)right);

                case longValue when rightPrio == intValue:
                    return (long)left - (int)right;

                case doubleValue when rightPrio == floatValue:
                    return (double)((double)left - (float)right);
               case doubleValue when rightPrio == intValue:
                    return (double)((double)left - (int)right);
                default:
                    throw new($"Invalid type {left.GetType().ToString()[7..]} for binary operand with {right.GetType().ToString()[7..]}");
            }
        }
  
        public static object Multiplication(object left, object right)
        {
            ConvertBoolToInt(ref left, ref right);

            var leftPrio = SyntaxFacts.GetTypePriority(left);
            var rightPrio = SyntaxFacts.GetTypePriority(right);
            switch(leftPrio)
            {
                case intValue when rightPrio == leftPrio:
                    return (int)left * (int)right;
                case floatValue when rightPrio == leftPrio:
                    return (float)left * (float)right;
                case longValue when rightPrio == leftPrio:
                    return (long)left * (long)right;
                case doubleValue when rightPrio == leftPrio:
                    return (double)left * (double)right;

                case intValue when rightPrio == floatValue:
                    return (int)((int)left * (float)right);
                case intValue when rightPrio == longValue:
                    return (int)((int)left * (long)right);
                case intValue when rightPrio == doubleValue:
                    return (int)((int)left * (double)right);

                case floatValue when rightPrio == intValue:
                    return (float)((float)left * (int)right);
                case floatValue when rightPrio == doubleValue:
                    return (float)((float)left * (double)right);

                case longValue when rightPrio == intValue:
                    return (long)left * (int)right;

                case doubleValue when rightPrio == floatValue:
                    return (double)((double)left * (float)right);
                case doubleValue when rightPrio == intValue:
                    return (double)((double)left * (int)right);
                default:
                    throw new($"Invalid type {left.GetType().ToString()[7..]} for binary operand with {right.GetType().ToString()[7..]}");
            }
        }

        public static object Division(object left, object right)
        {
            ConvertBoolToInt(ref left, ref right);

            var leftPrio = SyntaxFacts.GetTypePriority(left);
            var rightPrio = SyntaxFacts.GetTypePriority(right);
            switch(leftPrio)
            {

                case intValue when rightPrio == leftPrio:
                    return (int)left / (int)right;
                case floatValue when rightPrio == leftPrio:
                    return (float)left / (float)right;
                case longValue when rightPrio == leftPrio:
                    return (long)left / (long)right;
                case doubleValue when rightPrio == leftPrio:
                    return (double)left / (double)right;

                case intValue when rightPrio == floatValue:
                    return (int)((int)left / (float)right);
                case intValue when rightPrio == longValue:
                    return (int)((int)left / (long)right);
                case intValue when rightPrio == doubleValue:
                    return (int)((int)left / (double)right);

                case floatValue when rightPrio == intValue:
                    return (float)((float)left / (int)right);
                case floatValue when rightPrio == doubleValue:
                    return (float)((float)left / (double)right);

                case longValue when rightPrio == intValue:
                    return (long)((long)left / (int)right);

                case doubleValue when rightPrio == floatValue:
                    return (double)((double)left / (float)right);
                case doubleValue when rightPrio == intValue:
                    return (double)((double)left / (int)right);
                default:
                    throw new($"Invalid type {left.GetType().ToString()[7..]} for binary operand with {right.GetType().ToString()[7..]}");
            }
         
        }
        public static bool LesserThan(object left, object right)
        {
            ConvertBoolToInt(ref left, ref right);

            var leftPrio = SyntaxFacts.GetTypePriority(left);
            var rightPrio = SyntaxFacts.GetTypePriority(right);
            switch(leftPrio)
            {
                case intValue when rightPrio == leftPrio:
                    return (int)left < (int)right;
                case floatValue when rightPrio == leftPrio:
                    return (float)left < (float)right;
                case longValue when rightPrio == leftPrio:
                    return (long)left < (long)right;
                case doubleValue when rightPrio == leftPrio:
                    return (double)left < (double)right;

                case intValue when rightPrio == floatValue:
                    return (int)left < (float)right;
                case intValue when rightPrio == longValue:
                    return (int)left < (long)right;
                case intValue when rightPrio == doubleValue:
                    return (int)left < (double)right;

                case floatValue when rightPrio == intValue:
                    return (float)left < (int)right;
                case floatValue when rightPrio == doubleValue:
                    return (float)left < (double)right;

                case longValue when rightPrio == intValue:
                    return (long)left < (int)right;

                case doubleValue when rightPrio == floatValue:
                    return (double)left < (float)right;
                case doubleValue when rightPrio == intValue:
                    return (double)left < (int)right;
                default:
                    throw new($"Invalid type {left.GetType().ToString()[7..]} for binary operand with {right.GetType().ToString()[7..]}");
            }
        }
  
        public static bool LesserThanEquals(object left, object right)
        {
            ConvertBoolToInt(ref left, ref right);

            var leftPrio = SyntaxFacts.GetTypePriority(left);
            var rightPrio = SyntaxFacts.GetTypePriority(right);
            switch(leftPrio)
            {
                case intValue when rightPrio == leftPrio:
                    return (int)left <= (int)right;
                case floatValue when rightPrio == leftPrio:
                    return (float)left <= (float)right;
                case longValue when rightPrio == leftPrio:
                    return (long)left <= (long)right;
                case doubleValue when rightPrio == leftPrio:
                    return (double)left <= (double)right;

                case intValue when rightPrio == floatValue:
                    return (int)left <= (float)right;
                case intValue when rightPrio == longValue:
                    return (int)left <= (long)right;
                case intValue when rightPrio == doubleValue:
                    return (int)left <= (double)right;

                case floatValue when rightPrio == intValue:
                    return (float)left <= (int)right;
                case floatValue when rightPrio == doubleValue:
                    return (float)left <= (double)right;

                case longValue when rightPrio == intValue:
                    return (long)left <= (int)right;
                
                case doubleValue when rightPrio == floatValue:
                    return (double)left <= (float)right;
                case doubleValue when rightPrio == intValue:
                    return (double)left <= (int)right;
                default:
                    throw new($"Invalid type {left.GetType().ToString()[7..]} for binary operand with {right.GetType().ToString()[7..]}");
            }
        }

        public static bool GreaterThan(object left, object right)
        {
            ConvertBoolToInt(ref left, ref right);

            var leftPrio = SyntaxFacts.GetTypePriority(left);
            var rightPrio = SyntaxFacts.GetTypePriority(right);
            switch(leftPrio)
            {
                case intValue when rightPrio == leftPrio:
                    return (int)left > (int)right;
                case floatValue when rightPrio == leftPrio:
                    return (float)left > (float)right;
                case longValue when rightPrio == leftPrio:
                    return (long)left > (long)right;
                case doubleValue when rightPrio == leftPrio:
                    return (double)left > (double)right;

                case intValue when rightPrio == floatValue:
                    return (int)left > (float)right;
                case intValue when rightPrio == longValue:
                    return (int)left > (long)right;
                case intValue when rightPrio == doubleValue:
                    return (int)left > (double)right;

                case floatValue when rightPrio == intValue:
                    return (float)left > (int)right;
                case floatValue when rightPrio == doubleValue:
                    return (float)left > (double)right;

                 case longValue when rightPrio == intValue:
                    return (long)left > (int)right;

                case doubleValue when rightPrio == floatValue:
                    return (double)left > (float)right;
                case doubleValue when rightPrio == intValue:
                    return (double)left > (int)right;
                default:
                    throw new($"Invalid type {left.GetType().ToString()[7..]} for binary operand with {right.GetType().ToString()[7..]}");
            }
        }
        public static bool GreaterThanEquals(object left, object right)
        {
            ConvertBoolToInt(ref left, ref right);

            var leftPrio = SyntaxFacts.GetTypePriority(left);
            var rightPrio = SyntaxFacts.GetTypePriority(right);
            switch(leftPrio)
            {
                case intValue when rightPrio == leftPrio:
                    return (int)left >= (int)right;
                case floatValue when rightPrio == leftPrio:
                    return (float)left >= (float)right;
                case longValue when rightPrio == leftPrio:
                    return (long)left >= (long)right;
                case doubleValue when rightPrio == leftPrio:
                    return (double)left >= (double)right;

                case intValue when rightPrio == floatValue:
                    return (int)left >= (float)right;
                case intValue when rightPrio == longValue:
                    return (int)left >= (long)right;
                case intValue when rightPrio == doubleValue:
                    return (int)left >= (double)right;

                case floatValue when rightPrio == intValue:
                    return (float)left >= (int)right;
                case floatValue when rightPrio == doubleValue:
                    return (float)left >= (double)right;

                case longValue when rightPrio == intValue:
                    return (long)left >= (int)right;

                case doubleValue when rightPrio == floatValue:
                    return (double)left >= (float)right;
                case doubleValue when rightPrio == intValue:
                    return (double)left >= (int)right;
                default:
                    throw new($"Invalid type {left.GetType().ToString()[7..]} for binary operand with {right.GetType().ToString()[7..]}");
            }
        }
  
        public static object Pow(object left, object right)
        {
            ConvertBoolToInt(ref left, ref right);
            var leftPrio = SyntaxFacts.GetTypePriority(left);
            var rightPrio = SyntaxFacts.GetTypePriority(right);
            switch(leftPrio)
            {
                case intValue when rightPrio == leftPrio:
                    return (int)Math.Pow((int)left, (int)right);
                case floatValue when rightPrio == leftPrio:
                    return (float)Math.Pow((float)left, (float)right);
                case longValue when rightPrio == leftPrio:
                    return (long)BigInteger.Pow((long)left, Convert.ToInt32(right));
                case doubleValue when rightPrio == leftPrio:
                    return Math.Pow((double)left, (double)right);

                case intValue when rightPrio == floatValue:
                    return (int)Math.Pow((int)left, (float)right);
                case intValue when rightPrio == longValue:
                    return (int)Math.Pow((int)left, (long)right);
                case intValue when rightPrio == doubleValue:
                    return (int)Math.Pow((int)left, (double)right);

                case floatValue when rightPrio == intValue:
                    return (float)Math.Pow((float)left, (int)right);
                case floatValue when rightPrio == doubleValue:
                    return (float)Math.Pow((float)left, (double)right);

                case longValue when rightPrio == intValue:
                    return (long)BigInteger.Pow((long)left, (int)right);

                case doubleValue when rightPrio == floatValue:
                    return Math.Pow((double)left, (float)right);
                case doubleValue when rightPrio == intValue:
                    return Math.Pow((double)left, (int)right);
                default:
                    throw new($"Invalid type {left.GetType().ToString()[7..]} for binary operand with {right.GetType().ToString()[7..]}");
            }
        }
        
        public static object BitwiseOR(object left, object right)
        {
            if(left is bool)
            {
                return (bool)left | (bool)right;
            }
            return (int)left | (int)right;
        }

        public static object BitwiseAND(object left, object right)
        {
            if(left is bool || right is bool)
            {
                return (bool)left & (bool)right;
            }
            return (int)left & (int)right;
        }

        public static object BitwiseXOR(object left, object right)
        {
            if(left is bool || right is bool)
            {
                return (bool)left ^ (bool)right;
            }
            return (int)left ^ (int)right;
        }
        public static void ConvertBoolToInt(ref object left, ref object right)
        {
            int newRight = -1;
            int newLeft = -1;
            if (right is bool)
            {
                newRight = (bool)right ? 1 : 0;
            }
            if(left is bool)
            {
                newLeft = (bool)left ? 1 : 0;
            }
            left = newLeft >= 0 ? newLeft : left;
            right = newRight >= 0 ? newRight : right;
        }
    }
}
