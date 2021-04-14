using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.CodeAnalysis.Syntax;
using Compiler.CodeAnalysis.Parsing;
using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Syntax.Expression;
using Compiler.Logging;
using Compiler.CodeAnalysis.Text;


namespace Compiler.CodeAnalysis
{
    public sealed class Compilation
    {
        public Compilation(SyntaxTree syntax)
        {
            Syntax = syntax;
        }

        public SyntaxTree Syntax { get; }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {
            var binder = new Binder(variables);
            var boundExpression = binder.BindExpression(Syntax.Root);

            var diagnostics = Syntax.Diagnostics.Concat(binder.Diagnostics).ToArray();

            if(diagnostics.Any())
            {
                return new(diagnostics, null);
            }

            var evaluator = new Evaluator(boundExpression, variables);
            var value = evaluator.Evaluate();
            return new(Array.Empty<Diagnostic>(), value);
        }
    }
}
