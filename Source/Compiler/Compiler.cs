using System;
using Compiler.CodeAnalysis.Lexing;
using Compiler.CodeAnalysis.Syntax.Expression;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Compiler.CodeAnalysis.Syntax;
using Compiler.CodeAnalysis.Parsing;
using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis;
using Compiler.CodeAnalysis.Text;

namespace Compiler
{
    public static class Compiler
    {
        public static void Emit(string[] args)
        {
            var showTree = false;
            if(args.Length == 0)
            {
                Console.WriteLine("You must specify a file, or an input string");
                return;
            }
            
            var text = OpenFile(args[0]);

            var variables = new Dictionary<VariableSymbol, object>();


            //Looping over the args to check if they want to show the tree
            for(int i = 1; i < args.Length; i++)
            {
                
                //Using a switch statement here for future proofing!
                switch(args[i].ToUpper())
                {
                    case "--#SHOWTREE":
                        showTree = true;
                        Console.WriteLine(showTree ? "Now showing syntax tree" : "No longer showing syntax tree"); 
                        break;
                    default:
                        break;
                }
            }

            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = new Compilation(syntaxTree);

            var result = compilation.Evaluate(variables);



            //We concat the binder's diagnostics, and the syntax tree's diagnostics in case of ANY errors
            var diagnostics = result.Diagnostics;            
            //Displaying the tree if the program is ran with --#showTree
            if(showTree)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Parser.PrettyPrint(syntaxTree.Root);
                Console.ResetColor();    
            }

            //If there are any diagnostics, we print them in red
            if(!diagnostics.Any())
            {
                //Print out the results if there are no diagnostics
                Console.WriteLine(result.Value);
            } 
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                
                Console.WriteLine();
                //Literally just looping over each diagnostic,
                //to give us all of the errors
                foreach (var diag in diagnostics)
                {
                    Console.WriteLine(diag);
                }
                //Reset the color so that it doesn't look bad
                Console.ResetColor(); 

            }
        
        }
        
        //As the title would suggest, it opens a file, and returns it's contents
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
            catch (Exception)
            {
                // We can catch it, and allow for a an input such as 
                // `dotnet run (2 + 2)`
                return filePath;
            }
        }
    }
}
