using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Text;

namespace Uranium.CodeAnalysis.Binding
{
    internal sealed class BoundScope
    {
        private readonly Dictionary<string, VariableSymbol> _variables = new();

        public BoundScope? Parent { get; }

        public BoundScope(BoundScope? parent)
        {
            Parent = parent;
        }

        public bool TryDeclare(VariableSymbol variable)
        {
            if(_variables.ContainsKey(variable.Name))
            {
                return false;
            }

            _variables.Add(variable.Name, variable);
            return true;
        }

        //If I wanted to be fancy I could do something like
        //
        //public bool TryLookup(string name, out VariableSymbol variable)
        //    => _variables.TryGetValue(name, out variable) ? true : Parent?.TryLookup(name, out variable) ?? false;
        //
        //But that's just not needed
        public bool TryLookup(string name, out VariableSymbol variable)
        {
            if(_variables.TryGetValue(name, out variable!))
            {
                return true;
            }
            return Parent?.TryLookup(name, out variable) ?? false;
        }

        public ImmutableArray<VariableSymbol> GetDeclaredVariables() => _variables.Values.ToImmutableArray();

    }
}
