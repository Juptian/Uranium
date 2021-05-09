using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Collections.Immutable;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis;
using Uranium.CodeAnalysis.Text;
using Uranium.Logging;
using Uranium.CodeAnalysis.Symbols;

#pragma warning disable CS8618
#pragma warning disable CA2211

namespace Uranium
{
    public static class Uranium
    {
        private static bool _showTree = false;
        private static bool _showBoundTree = false;
        private static SyntaxTree _syntaxTree;
        private static Compilation? _previous = null;

        public static ImmutableArray<Diagnostic> Diagnostics;

        public static bool Emit(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("You must specify a file, or an input string");
                return false;
            }
            else if (args.Length >= 1)
            {
                ReadArgs(args);
            }

            var _text = OpenFile(args[0]);
            var variables = new Dictionary<VariableSymbol, object>();
            _syntaxTree = SyntaxTree.Parse(_text);

            var compilation = _previous?.ContinueWith(_syntaxTree) ?? new Compilation(_syntaxTree);

            _previous = compilation;

            
            if(_showTree)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                _syntaxTree.Root.WriteTo(Console.Out);
                Console.ResetColor();    
            }
            if(_showBoundTree)
            {
                compilation.EmitTree(Console.Out);
            }

            var result = compilation.Evaluate(variables);
            result.DealWithDiagnostics();
            
            return true;
        }

        private static void ReadArgs(string[] args)
        {
            for(int i = 1; i < args.Length; i++)
            {
                //Using a switch statement here for future proofing!
                switch(args[i].ToUpper())
                {
                    case "--SHOWTREE":
                        _showTree = true;
                        break;
                    case "--BOUNDTREE":
                        _showBoundTree = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private static void DealWithDiagnostics(this EvaluationResult result)
        {
            var diagnostics = result.Diagnostics;
            Diagnostics = diagnostics;
            //If there are any diagnostics, we print them in red
            //This makes it more easy to determine what is and isn't an error
            if(!diagnostics.Any())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(result.Value);
                Console.ResetColor();
            } 
            else
            {
                var treeText = _syntaxTree.Text;
                Console.ForegroundColor = ConsoleColor.Red;
                
                //Whitespace to divide the errors more clearly from everything else 
                Console.WriteLine();

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
            Console.WriteLine($"({lineNumber}, {character}), {diag}");
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
