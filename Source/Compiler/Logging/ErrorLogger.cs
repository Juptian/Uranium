using System;
using System.Runtime.CompilerServices;

namespace Compiler.Logging
{
    public static class ErrorLogger
    {
        public static void ReportInvalidNumber([CallerLineNumber] int position = 0)
        {
            Console.Error.WriteLine("JC0001: Error at index: " + position + @" 
Cannot have an invalid number, this includes having multiple .'s in a number, or a number that is too large!");
        }
        public static void ReportNumberStartingWith_([CallerLineNumber] int position = 0)
        {
            Console.Error.WriteLine("JC0002: Error at index: " + position + @"
Cannot have a number starting with an underscore (_) !");
        }

        public static void ReportUnfinishedMultiLineComment([CallerLineNumber] int position = 0)
        {
            Console.Error.WriteLine("JCOOO3: Error at index: " + position + @"
Unfinished multiline comment!");
        }
    }
}
