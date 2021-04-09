using System;
using Compiler;

namespace Main
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Compiler.Compiler.Emit(args);
        }
    }
}
