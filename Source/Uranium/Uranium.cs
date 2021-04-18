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
using Uranium.Logging;

namespace Uranium
{
    public static class Uranium
    {
        private static bool _showTree = false;
        private static SyntaxTree _syntaxTree;
        private static Compilation? _previous = null;
        public static void Emit(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("You must specify a file, or an input string");
                return;
            }
            else if (args.Length >= 1)
            {
                ReadArgs(args);
            }

            var _text = OpenFile(args[0]);
            var variables = new Dictionary<VariableSymbol, object>();

            //This is the important line
            //It gets shit into motion
            _syntaxTree = SyntaxTree.Parse(_text);
            //Then we make a compilation
            //the _previous?.ContinueWith(_syntaxTree) ?? new Compilation(_syntaxTree);
            //breaks down to:
            //  _previous may be null
            //  So we try to continue with it.
            //  If _previous is null null, just make a new compilation
            var compilation = _previous?.ContinueWith(_syntaxTree) ?? new Compilation(_syntaxTree);

            //We assign previous here
            _previous = compilation;

            var result = compilation.Evaluate(variables);
            result.DealWithDiagnostics();
            
            //Displaying the tree if the program is ran with --#_showTree
            if(_showTree)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                _syntaxTree.Root.WriteTo(Console.Out);
                Console.ResetColor();    
            }
        }

        private static void ReadArgs(string[] args)
        {
            //Looping over the args to check if they want to show the tree
            for(int i = 1; i < args.Length; i++)
            {
                //Using a switch statement here for future proofing!
                switch(args[i].ToUpper())
                {
                    case "--#SHOWTREE":
                        _showTree = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private static void DealWithDiagnostics(this EvaluationResult result)
        {
            //We concat the binder's diagnostics, and the syntax tree's diagnostics in case of ANY errors
            var diagnostics = result.Diagnostics;   

            //If there are any diagnostics, we print them in red
            if(!diagnostics.Any())
            {
                //Print out the results if there are no diagnostics
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(result.Value);
                Console.ResetColor();
            } 
            else
            {
                var treeText = _syntaxTree.Text;
                Console.ForegroundColor = ConsoleColor.Red;
                
                Console.WriteLine();
                //Literally just looping over each diagnostic,
                //to give us all of the errors
                foreach (var diag in diagnostics)
                {
                    PrintDiagnostic(diag, treeText);
                }
                //Reset the color so that it doesn't look bad
                Console.ResetColor(); 
            }
        }

        private static void PrintDiagnostic(Diagnostic diag, SourceText treeText)
        {
            var lineIndex = treeText.GetLineIndex(diag.Span.Start);
            var lineNumber = lineIndex + 1;
            var character = diag.Span.Start - treeText.Lines[lineIndex].Start + 1;

            Console.WriteLine();
            Console.Write($"({lineNumber}, {character}),");
            Console.WriteLine($"{diag}");
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
            catch (Exception)
            {
                // We can catch it, and allow for a an input such as 
                // `dotnet run (2 + 2)`
                return filePath;
            }
        }
    }
}
