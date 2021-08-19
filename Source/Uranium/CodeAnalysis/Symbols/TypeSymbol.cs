using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Symbols
{
    public sealed class TypeSymbol : Symbol
    {
        public static readonly TypeSymbol Int = new("int");
        public static readonly TypeSymbol Long = new("long");
        public static readonly TypeSymbol Double = new("double");
        public static readonly TypeSymbol Float = new("float");
        public static readonly TypeSymbol String = new("string");
        public static readonly TypeSymbol Char = new("char");
        public static readonly TypeSymbol Bool = new("bool");
        public static readonly TypeSymbol Error = new("Error");
        public static readonly TypeSymbol Void = new("void");

        private TypeSymbol(string name)
            : base(name)
        { }

        public override SymbolKind Kind => SymbolKind.Type; 
    }
}
