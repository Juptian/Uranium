using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport
{
    internal static class EvaluatorPow
    {
        public static object Pow(object @base, object power)
        {
            int newPowerInt = 0;
            long newPowerLong = 0;
            if(EvaluatorEquals.IsFloatingPointNumber(power.GetType()))
            {
                if(power.GetType() == typeof(float))
                {
                    newPowerInt = (int)power;
                }
                else
                {
                    newPowerLong = (long)power;
                }
            } 
            else if(power.GetType() == typeof(int))
            {
                newPowerInt = (int)power;
            }
            else
            {
                newPowerLong = (long)power;
            }

            if(newPowerInt > 0)
            {
                return PowInt((int)@base, newPowerInt);
            }
            else if (newPowerLong > 0)
            {
                return PowLong((long)@base, (long)newPowerLong);
            }
            throw new($"Unkown type for pow multiplication {power.GetType()}");
        }

        private static int PowInt(int number, int power)
        {
            int result = number;
            for(; power > 1; power--)
            {
                result *= number;
            }
            return result;
        }

        private static long PowLong(long number, long power)
        {
            long result = number;
            for(; power > 1; power--)
            {
                result *= number;
            }
            return result;
        }

    }
}
