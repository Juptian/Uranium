using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.CodeAnalysis.Syntax;
using Compiler.CodeAnalysis.Parsing;
using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Syntax.Expression;


namespace Compiler.CodeAnalysis
{
    public sealed class Compilation
    {
        public Compilation(SyntaxTree syntax)
        {
            Syntax = syntax;
        }

        public SyntaxTree Syntax { get; }

        public EvaluationResult Evaluate()
        {
            var binder = new Binder();
            var boundExpression = binder.BindExpression(Syntax.Root);

            var diagnostics = Syntax.Diagnostics.Concat(binder.Diagnostics).ToArray();

            if(diagnostics.Any())
            {
                return new(diagnostics, null);
            }

            var evaluator = new Evaluator(boundExpression);
            var value = evaluator.Evaluate();
            return new(Array.Empty<string>(), value);
        }
    }
}
