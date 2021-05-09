using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Symbols
{
    public abstract class Symbol
    {
        private protected Symbol(string name)
        {
            Name = name;
        }
        public string Name { get; }
        public abstract SymbolKind Kind { get; }

        public override string ToString()
            => Name;
    }
}
