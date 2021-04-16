﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Parsing;
using Uranium.CodeAnalysis.Binding;
using Uranium.CodeAnalysis.Syntax.Expression;
using Uranium.Logging;
using Uranium.CodeAnalysis.Text;


namespace Uranium.CodeAnalysis
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

            var diagnostics = Syntax.Diagnostics.Concat(binder.Diagnostics).ToImmutableArray();

            if(diagnostics.Any())
            {
                return new(diagnostics, null);
            }

            var evaluator = new Evaluator(boundExpression, variables);
            var value = evaluator.Evaluate();
            return new(ImmutableArray<Diagnostic>.Empty, value);
        }
    }
}
