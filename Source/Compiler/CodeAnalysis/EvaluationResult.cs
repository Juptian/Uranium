using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Syntax.Expression;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis
{
    public sealed class EvaluationResult
    {
        public EvaluationResult(IEnumerable<string> diagnostics, object value)
        {
            Diagnostics = diagnostics.ToArray();
            Value = value;
        }

        public IReadOnlyList<string> Diagnostics { get; }
        public object Value { get; }
    }
}
