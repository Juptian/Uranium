using System;
using Compiler;
using Compiler.Tests;
namespace Main
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Tests tests = new();
            tests.RunTests();

            Compiler.Compiler.Emit(args);
        }
    }
}
