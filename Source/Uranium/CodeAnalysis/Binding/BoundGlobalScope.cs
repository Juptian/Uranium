using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.Logging;
using Uranium.CodeAnalysis.Text;
using Uranium.CodeAnalysis.Binding.NodeKinds;
using Uranium.CodeAnalysis.Binding.Statements;
using Uranium.CodeAnalysis.Symbols;


namespace Uranium.CodeAnalysis.Binding
{
    internal sealed class BoundGlobalScope
    {
        public BoundGlobalScope(
            BoundGlobalScope? previous,
            ImmutableArray<Diagnostic> diagnostics, 
            ImmutableArray<VariableSymbol> variables, 
            BoundStatement statement)
        {
            Previous = previous;
            Diagnostics = diagnostics;
            Variables = variables;
            Statement = statement;
        }

        public BoundGlobalScope? Previous { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ImmutableArray<VariableSymbol> Variables { get; }
        public BoundStatement Statement { get; }
    }
}
