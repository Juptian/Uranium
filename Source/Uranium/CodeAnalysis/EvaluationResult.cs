using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Binding;
using Uranium.CodeAnalysis.Syntax.Expression;
using Uranium.CodeAnalysis.Syntax;
using Uranium.Logging;

namespace Uranium.CodeAnalysis
{
    public sealed class EvaluationResult
    {
        public EvaluationResult(ImmutableArray<Diagnostic> diagnostics, object? value)
        {
            Diagnostics = diagnostics;
            Value = value;
        }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public object? Value { get; }
    }
}
