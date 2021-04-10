using System;
using System.Runtime.CompilerServices;

namespace Compiler.Logging
{
    public static class ErrorLogger
    {
        public static void ReportInvalidNumber([CallerLineNumber] int source = 0)
        {
            throw new Exception("JC0001: Error at index: " + source + @" 
Cannot have an invalid number, this includes having multiple .'s in a number, or a number that is too large!");
        }
    }
}
