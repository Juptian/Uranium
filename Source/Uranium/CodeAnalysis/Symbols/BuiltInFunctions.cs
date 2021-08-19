using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Symbols
{
    internal static class BuiltInFunctions
    {
        public static readonly FunctionSymbol Print = new("print", ImmutableArray.Create(new ParameterSymbol("text", TypeSymbol.String)), TypeSymbol.Void);
        public static readonly FunctionSymbol Println = new("println", ImmutableArray.Create(new ParameterSymbol("Text", TypeSymbol.String)), TypeSymbol.Void);
        public static readonly FunctionSymbol Input = new("input", ImmutableArray<ParameterSymbol>.Empty, TypeSymbol.String);

        internal static IEnumerable<FunctionSymbol?> GetAll()
            => typeof(BuiltInFunctions)
                    .GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.FieldType == typeof(FunctionSymbol))
                    .Select(f => (FunctionSymbol?)f.GetValue(null));
    }
}
