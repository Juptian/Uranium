using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Text
{
    public sealed class VariableSymbol
    {
        internal VariableSymbol(string name, bool isReadOnly, Type type)
        {
            Name = name;
            IsReadOnly = isReadOnly;
            Type = type;
        }

        public string Name { get; }
        public bool IsReadOnly { get; }
        public Type Type { get; }

        public override string ToString()
            => $"{Name}, {Type.ToString()[7..]}";
    }
}
