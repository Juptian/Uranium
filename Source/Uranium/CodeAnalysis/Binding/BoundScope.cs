using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Symbols;
using Uranium.CodeAnalysis.Text;

namespace Uranium.CodeAnalysis.Binding
{
    internal sealed class BoundScope
    {
        private readonly Dictionary<string, VariableSymbol> _variables = new();
        private readonly Dictionary<string, FunctionSymbol> _functions = new();

        public BoundScope? Parent { get; }

        public BoundScope(BoundScope? parent)
        {
            Parent = parent;
        }

        public bool TryDeclareVariable(VariableSymbol variable)
        {
            if(_variables.ContainsKey(variable.Name) || variable.Name == "?")
            {
                return false;
            }

            _variables.Add(variable.Name, variable);
            return true;
        }

        public bool TryLookupVariable(string name, out VariableSymbol variable)
        {
            if(_variables.TryGetValue(name, out variable!))
            {
                return true;
            }
            return Parent?.TryLookupVariable(name, out variable) ?? false;
        }

        public bool TryDeclareFunction(FunctionSymbol function)
        {
            if(_functions.ContainsKey(function.Name))
            {
                return false;
            }

            _functions.Add(function.Name, function);
            return true;
        }

        public bool TryLookupFunction(string name, out FunctionSymbol function)
        {
            if(_functions.TryGetValue(name, out function!))
            {
                return true;
            }
            return Parent?.TryLookupFunction(name, out function) ?? false;
        }


        public ImmutableArray<VariableSymbol> GetDeclaredVariables() => _variables.Values.ToImmutableArray();
        public ImmutableArray<FunctionSymbol> GetDeclaredFunctions() => _functions.Values.ToImmutableArray();

    }
}
