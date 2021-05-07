using System;
using System.Numerics;

#pragma warning disable IDE0066 // Convert switch statement to expressio
// Every time I've tried I've failed for an unkown reason
namespace Uranium.CodeAnalysis.Syntax.EvaluatorSupport
{
    internal static class Operations
    {
        private const int intValue = 1;
        public static object Addition(object left, object right)
        {
            var leftPrio = SyntaxFacts.GetTypePriority(left);
            var rightPrio = SyntaxFacts.GetTypePriority(right);
            switch(leftPrio)
            {
                case intValue when rightPrio == leftPrio:
                    return (int)left + (int)right;
                case intValue + 1 when rightPrio == leftPrio:
                    return (float)left + (float)right;
                case intValue + 2 when rightPrio == leftPrio:
                    return (long)left + (long)right;
                case intValue + 3 when rightPrio == leftPrio:
                    return (double)left + (double)right;

                case intValue when rightPrio == intValue + 1:
                    return (int)((int)left + (float)right);
                case intValue when rightPrio == intValue + 2:
                    return (int)((int)left + (long)right);
                case intValue when rightPrio == intValue + 3:
                    return (int)((int)left + (double)right);

                case intValue + 1 when rightPrio == intValue:
                    return (float)((float)left + (int)right);
                case intValue + 1 when rightPrio == intValue + 2:
                    return (float)((float)left + (long)right);
                case intValue + 1 when rightPrio == intValue + 3:
                    return (float)((float)left + (double)right);

                case intValue + 2 when rightPrio == intValue + 1:
                    return (long)((long)left + (float)right);
                case intValue + 2 when rightPrio == intValue:
                    return (long)((long)left + (int)right);
                case intValue + 2 when rightPrio == intValue + 3:
                    return (long)((long)left + (double)right);

                case intValue + 3 when rightPrio == intValue + 1:
                    return (double)((double)left + (float)right);
                case intValue + 3 when rightPrio == intValue + 2:
                    return (double)((double)left + (long)right);
                case intValue + 3 when rightPrio == intValue:
                    return (double)((double)left + (int)right);
                default:
                    throw new($"Invalid type {left.GetType()} for binary operand with {right.GetType()}");
            }
            
        }
        public static object Subtraction(object left, object right)
        {
            var leftPrio = SyntaxFacts.GetTypePriority(left);
            var rightPrio = SyntaxFacts.GetTypePriority(right);
            switch(leftPrio)
            {
                case intValue when rightPrio == leftPrio:
                    return (int)left - (int)right;
                case intValue + 1 when rightPrio == leftPrio:
                    return (float)left - (float)right;
                case intValue + 2 when rightPrio == leftPrio:
                    return (long)left - (long)right;
                case intValue + 3 when rightPrio == leftPrio:
                    return (double)left - (double)right;

                case intValue when rightPrio == intValue + 1:
                    return (int)((int)left - (float)right);
                case intValue when rightPrio == intValue + 2:
                    return (int)((int)left - (long)right);
                case intValue when rightPrio == intValue + 3:
                    return (int)((int)left - (double)right);

                case intValue + 1 when rightPrio == intValue:
                    return (float)((float)left - (int)right);
                case intValue + 1 when rightPrio == intValue + 2:
                    return (float)((float)left - (long)right);
                case intValue + 1 when rightPrio == intValue + 3:
                    return (float)((float)left - (double)right);

                case intValue + 2 when rightPrio == intValue + 1:
                    return (long)((long)left - (float)right);
                case intValue + 2 when rightPrio == intValue:
                    return (long)((long)left - (int)right);
                case intValue + 2 when rightPrio == intValue + 3:
                    return (long)((long)left - (double)right);

                case intValue + 3 when rightPrio == intValue + 1:
                    return (double)((double)left - (float)right);
               case intValue + 3 when rightPrio == intValue + 2:
                   return (double)((double)left - (long)right);
               case intValue + 3 when rightPrio == intValue:
                    return (double)((double)left - (int)right);
                default:
                    throw new($"Invalid type {left.GetType()} for binary operand with {right.GetType()}");
            }
        }
  
        public static object Multiplication(object left, object right)
        {
            var leftPrio = SyntaxFacts.GetTypePriority(left);
            var rightPrio = SyntaxFacts.GetTypePriority(right);
            switch(leftPrio)
            {
                case intValue when rightPrio == leftPrio:
                    return (int)left * (int)right;
                case intValue + 1 when rightPrio == leftPrio:
                    return (float)left * (float)right;
                case intValue + 2 when rightPrio == leftPrio:
                    return (long)left * (long)right;
                case intValue + 3 when rightPrio == leftPrio:
                    return (double)left * (double)right;

                case intValue when rightPrio == intValue + 1:
                    return (int)((int)left * (float)right);
                case intValue when rightPrio == intValue + 2:
                    return (int)((int)left * (long)right);
                case intValue when rightPrio == intValue + 3:
                    return (int)((int)left * (double)right);

                case intValue + 1 when rightPrio == intValue:
                    return (float)((float)left * (int)right);
                case intValue + 1 when rightPrio == intValue + 2:
                    return (float)((float)left * (long)right);
                case intValue + 1 when rightPrio == intValue + 3:
                    return (float)((float)left * (double)right);

                case intValue + 2 when rightPrio == intValue + 1:
                    return (long)((long)left * (float)right);
                case intValue + 2 when rightPrio == intValue:
                    return (long)((long)left * (int)right);
                case intValue + 2 when rightPrio == intValue + 3:
                    return (long)((long)left * (double)right);

                case intValue + 3 when rightPrio == intValue + 1:
                    return (double)((double)left * (float)right);
                case intValue + 3 when rightPrio == intValue + 2:
                    return (double)((double)left * (long)right);
                case intValue + 3 when rightPrio == intValue:
                    return (double)((double)left * (int)right);
                default:
                    throw new($"Invalid type {left.GetType()} for binary operand with {right.GetType()}");
            }
        }

        public static object Division(object left, object right)
        {
            var leftPrio = SyntaxFacts.GetTypePriority(left);
            var rightPrio = SyntaxFacts.GetTypePriority(right);
            switch(leftPrio)
            {

                case intValue when rightPrio == leftPrio:
                    return (int)left / (int)right;
                case intValue + 1 when rightPrio == leftPrio:
                    return (float)left / (float)right;
                case intValue + 2 when rightPrio == leftPrio:
                    return (long)left / (long)right;
                case intValue + 3 when rightPrio == leftPrio:
                    return (double)left / (double)right;

                case intValue when rightPrio == intValue + 1:
                    return (int)((int)left / (float)right);
                case intValue when rightPrio == intValue + 2:
                    return (int)((int)left / (long)right);
                case intValue when rightPrio == intValue + 3:
                    return (int)((int)left / (double)right);

                case intValue + 1 when rightPrio == intValue:
                    return (float)((float)left / (int)right);
                case intValue + 1 when rightPrio == intValue + 2:
                    return (float)((float)left / (long)right);
                case intValue + 1 when rightPrio == intValue + 3:
                    return (float)((float)left / (double)right);

                case intValue + 2 when rightPrio == intValue + 1:
                    return (long)((long)left / (float)right);
                case intValue + 2 when rightPrio == intValue:
                    return (long)((long)left / (int)right);
                case intValue + 2 when rightPrio == intValue + 3:
                    return (long)((long)left / (double)right);

                case intValue + 3 when rightPrio == intValue + 1:
                    return (double)((double)left / (float)right);
                case intValue + 3 when rightPrio == intValue + 2:
                    return (double)((double)left / (long)right);
                case intValue + 3 when rightPrio == intValue:
                    return (double)((double)left / (int)right);
                default:
                    throw new($"Invalid type {left.GetType()} for binary operand with {right.GetType()}");
            }
         
        }
        public static bool LesserThan(object left, object right)
        {
            var leftPrio = SyntaxFacts.GetTypePriority(left);
            var rightPrio = SyntaxFacts.GetTypePriority(right);
            switch(leftPrio)
            {
                case intValue when rightPrio == leftPrio:
                    return (int)left < (int)right;
                case intValue + 1 when rightPrio == leftPrio:
                    return (float)left < (float)right;
                case intValue + 2 when rightPrio == leftPrio:
                    return (long)left < (long)right;
                case intValue + 3 when rightPrio == leftPrio:
                    return (double)left < (double)right;

                case intValue when rightPrio == intValue + 1:
                    return (int)left < (float)right;
                case intValue when rightPrio == intValue + 2:
                    return (int)left < (long)right;
                case intValue when rightPrio == intValue + 3:
                    return (int)left < (double)right;

                case intValue + 1 when rightPrio == intValue:
                    return (float)left < (int)right;
                case intValue + 1 when rightPrio == intValue + 2:
                    return (float)left < (long)right;
                case intValue + 1 when rightPrio == intValue + 3:
                    return (float)left < (double)right;

                case intValue + 2 when rightPrio == intValue + 1:
                    return (long)left < (float)right;
                case intValue + 2 when rightPrio == intValue:
                    return (long)left < (int)right;
                case intValue + 2 when rightPrio == intValue + 3:
                    return (long)left < (double)right;

                case intValue + 3 when rightPrio == intValue + 1:
                    return (double)left < (float)right;
                case intValue + 3 when rightPrio == intValue + 2:
                    return (double)left < (long)right;
                case intValue + 3 when rightPrio == intValue:
                    return (double)left < (int)right;
                default:
                    throw new($"Invalid type {left.GetType()} for binary operand with {right.GetType()}");
            }
        }
  
        public static bool LesserThanEquals(object left, object right)
        {
            var leftPrio = SyntaxFacts.GetTypePriority(left);
            var rightPrio = SyntaxFacts.GetTypePriority(right);
            switch(leftPrio)
            {
                case intValue when rightPrio == leftPrio:
                    return (int)left <= (int)right;
                case intValue + 1 when rightPrio == leftPrio:
                    return (float)left <= (float)right;
                case intValue + 2 when rightPrio == leftPrio:
                    return (long)left <= (long)right;
                case intValue + 3 when rightPrio == leftPrio:
                    return (double)left <= (double)right;

                case intValue when rightPrio == intValue + 1:
                    return (int)left <= (float)right;
                case intValue when rightPrio == intValue + 2:
                    return (int)left <= (long)right;
                case intValue when rightPrio == intValue + 3:
                    return (int)left <= (double)right;

                case intValue + 1 when rightPrio == intValue:
                    return (float)left <= (int)right;
                case intValue + 1 when rightPrio == intValue + 2:
                    return (float)left <= (long)right;
                case intValue + 1 when rightPrio == intValue + 3:
                    return (float)left <= (double)right;

                case intValue + 2 when rightPrio == intValue + 1:
                    return (long)left <= (float)right;
                case intValue + 2 when rightPrio == intValue:
                    return (long)left <= (int)right;
                case intValue + 2 when rightPrio == intValue + 3:
                    return (long)left <= (double)right;

                case intValue + 3 when rightPrio == intValue + 1:
                    return (double)left <= (float)right;
                case intValue + 3 when rightPrio == intValue + 2:
                    return (double)left <= (long)right;
                case intValue + 3 when rightPrio == intValue:
                    return (double)left <= (int)right;
                default:
                    throw new($"Invalid type {left.GetType()} for binary operand with {right.GetType()}");
            }
        }

        public static bool GreaterThan(object left, object right)
        {
                var leftPrio = SyntaxFacts.GetTypePriority(left);
                var rightPrio = SyntaxFacts.GetTypePriority(right);
                switch(leftPrio)
                {
                case intValue when rightPrio == leftPrio:
                    return (int)left > (int)right;
                case intValue + 1 when rightPrio == leftPrio:
                    return (float)left > (float)right;
                case intValue + 2 when rightPrio == leftPrio:
                    return (long)left > (long)right;
                case intValue + 3 when rightPrio == leftPrio:
                    return (double)left > (double)right;

                case intValue when rightPrio == intValue + 1:
                    return (int)left > (float)right;
                case intValue when rightPrio == intValue + 2:
                    return (int)left > (long)right;
                case intValue when rightPrio == intValue + 3:
                    return (int)left > (double)right;

                case intValue + 1 when rightPrio == intValue:
                    return (float)left > (int)right;
                case intValue + 1 when rightPrio == intValue + 2:
                    return (float)left > (long)right;
                case intValue + 1 when rightPrio == intValue + 3:
                    return (float)left > (double)right;

                case intValue + 2 when rightPrio == intValue + 1:
                    return (long)left > (float)right;
                 case intValue + 2 when rightPrio == intValue:
                    return (long)left > (int)right;
                case intValue + 2 when rightPrio == intValue + 3:
                    return (long)left > (double)right;

                case intValue + 3 when rightPrio == intValue + 1:
                    return (double)left > (float)right;
                case intValue + 3 when rightPrio == intValue + 2:
                    return (double)left > (long)right;
                case intValue + 3 when rightPrio == intValue:
                    return (double)left > (int)right;
                default:
                    throw new($"Invalid type {left.GetType()} for binary operand with {right.GetType()}");
            }
        }
        public static bool GreaterThanEquals(object left, object right)
        {
            var leftPrio = SyntaxFacts.GetTypePriority(left);
            var rightPrio = SyntaxFacts.GetTypePriority(right);
            switch(leftPrio)
            {
                case intValue when rightPrio == leftPrio:
                    return (int)left >= (int)right;
                case intValue + 1 when rightPrio == leftPrio:
                    return (float)left >= (float)right;
                case intValue + 2 when rightPrio == leftPrio:
                    return (long)left >= (long)right;
                case intValue + 3 when rightPrio == leftPrio:
                    return (double)left >= (double)right;

                case intValue when rightPrio == intValue + 1:
                    return (int)left >= (float)right;
                case intValue when rightPrio == intValue + 2:
                    return (int)left >= (long)right;
                case intValue when rightPrio == intValue + 3:
                    return (int)left >= (double)right;

                case intValue + 1 when rightPrio == intValue:
                    return (float)left >= (int)right;
                case intValue + 1 when rightPrio == intValue + 2:
                    return (float)left >= (long)right;
                case intValue + 1 when rightPrio == intValue + 3:
                    return (float)left >= (double)right;

                case intValue + 2 when rightPrio == intValue + 1:
                    return (long)left >= (float)right;
                case intValue + 2 when rightPrio == intValue:
                    return (long)left >= (int)right;
                case intValue + 2 when rightPrio == intValue + 3:
                    return (long)left >= (double)right;

                case intValue + 3 when rightPrio == intValue + 1:
                    return (double)left >= (float)right;
                case intValue + 3 when rightPrio == intValue + 2:
                    return (double)left >= (long)right;
                case intValue + 3 when rightPrio == intValue:
                    return (double)left >= (int)right;
                default:
                    throw new($"Invalid type {left.GetType()} for binary operand with {right.GetType()}");
            }
        }
  
        public static object Pow(object left, object right)
        {
            var leftPrio = SyntaxFacts.GetTypePriority(left);
            var rightPrio = SyntaxFacts.GetTypePriority(right);
            switch(leftPrio)
            {
                case intValue when rightPrio == leftPrio:
                    return (int)Math.Pow((int)left, (int)right);
                case intValue + 1 when rightPrio == leftPrio:
                    return (float)Math.Pow((float)left, (float)right);
                case intValue + 2 when rightPrio == leftPrio:
                    return (long)BigInteger.Pow((long)left, Convert.ToInt32(right));
                case intValue + 3 when rightPrio == leftPrio:
                    return Math.Pow((double)left, (double)right);

                case intValue when rightPrio == intValue + 1:
                    return (int)Math.Pow((int)left, (float)right);
                case intValue when rightPrio == intValue + 2:
                    return (int)Math.Pow((int)left, (long)right);
                case intValue when rightPrio == intValue + 3:
                    return (int)Math.Pow((int)left, (double)right);

                case intValue + 1 when rightPrio == intValue:
                    return (float)Math.Pow((float)left, (int)right);
                case intValue + 1 when rightPrio == intValue + 2:
                    return (float)Math.Pow((float)left, (long)right);
                case intValue + 1 when rightPrio == intValue + 3:
                    return (float)Math.Pow((float)left, (double)right);

                case intValue + 2 when rightPrio == intValue + 1:
                    return (long)BigInteger.Pow((long)left, (int)right);
                case intValue + 2 when rightPrio == intValue:
                    return (long)BigInteger.Pow((long)left, (int)right);
                case intValue + 2 when rightPrio == intValue + 3:
                    return (long)BigInteger.Pow((long)left, (int)right);

                case intValue + 3 when rightPrio == intValue + 1:
                    return Math.Pow((double)left, (float)right);
                case intValue + 3 when rightPrio == intValue + 2:
                    return Math.Pow((double)left, (long)right);
                case intValue + 3 when rightPrio == intValue:
                    return Math.Pow((double)left, (int)right);
                default:
                    throw new($"Invalid type {left.GetType()} for binary operand with {right.GetType()}");
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

    }
}
