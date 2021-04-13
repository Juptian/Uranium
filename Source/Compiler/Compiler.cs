using System;
using Compiler.Lexing;
using Compiler.Parsing;
using Compiler.Syntax.Expression;
using System.Linq;
using System.IO;

namespace Compiler
{
    public static class Compiler
    {
        public static void Emit(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("You must specify a file, or an input string");
                return;
            }
            
            var text = OpenFile(args[0]);
            var parser = new Parser(text);

            var syntaxTree = parser.Parse();

            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;

            Parser.PrettyPrint(syntaxTree.Root);

            Console.ForegroundColor = color;

            if(syntaxTree.Diagnostics.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (var diag in syntaxTree.Diagnostics)
                {
                    Console.WriteLine(diag);
                }
                Console.ForegroundColor = color;
            } 
            else
            {
                var evaluator = new Evaluator(syntaxTree.Root);
                var result = evaluator.Evaluate();
                Console.WriteLine(result);
            }

            //Lexer lexer = new( OpenFile(args[0]) );
            //lexer.NextToken();
        }

        private static string OpenFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new("File path must not be empty");
            }
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                // We can catch it, and allow for a an input such as 
                // `dotnet run (2 + 2)`
                return filePath;
            }
        }
    }
}
