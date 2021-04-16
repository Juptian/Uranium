using System;
using Uranium.CodeAnalysis.Lexing;
using Uranium.CodeAnalysis.Syntax.Expression;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Parsing;
using Uranium.CodeAnalysis.Binding;
using Uranium.CodeAnalysis;
using Uranium.CodeAnalysis.Text;

namespace Uranium
{
    public static class Uranium
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
                Console.ForegroundColor = ConsoleColor.Green;
                syntaxTree.Root.WriteTo(Console.Out);
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
                var treeText = syntaxTree.Text;
                Console.ForegroundColor = ConsoleColor.Red;
                
                Console.WriteLine();
                //Literally just looping over each diagnostic,
                //to give us all of the errors
                foreach (var diag in diagnostics)
                {
                    var lineIndex = treeText.GetLineIndex(diag.Span.Start);
                    var lineNumber = lineIndex + 1;
                    var character = diag.Span.Start - treeText.Lines[lineIndex].Start + 1;

                    Console.WriteLine();
                    Console.Write($"({lineNumber}, {character}),");
                    Console.WriteLine($"{diag}");

                    var prefix = text.Substring(0, diag.Span.Start);
                    var error = text.Substring(diag.Span.Start, diag.Span.Length);
                    var suffix = text.Substring(diag.Span.End);

                    Console.ResetColor();
                    Console.Write($"     {prefix}");

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(error);
                    Console.ResetColor();
                    Console.Write(suffix);
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
