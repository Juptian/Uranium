using System;
using Compiler.CodeAnalysis.Lexing;
using Compiler.CodeAnalysis.Syntax.Expression;
using System.Linq;
using System.IO;
using Compiler.CodeAnalysis.Syntax;
using Compiler.CodeAnalysis.Parsing;

namespace Compiler
{
    public static class Compiler
    {
        public static void Emit(string[] args)
        {
            bool showTree = false;
            if(args.Length == 0)
            {
                Console.WriteLine("You must specify a file, or an input string");
                return;
            }
            
            var text = OpenFile(args[0]);

            for(int i = 1; i < args.Length; i++)
            {
                Console.WriteLine(args[i]);
                if(args[i].Equals("--#SHOWTREE", StringComparison.OrdinalIgnoreCase))
                {
                    showTree ^= true;
                    Console.WriteLine(showTree ? "Now showing syntax tree" : "No longer showing syntax tree"); 
                }
            }

            var syntaxTree = SyntaxTree.Parse(text);
             var color = Console.ForegroundColor;
            
            if(showTree)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Parser.PrettyPrint(syntaxTree.Root);
                Console.ForegroundColor = color;
            }

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
