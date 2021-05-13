using System;
using Uranium.CodeAnalysis.Syntax;

namespace Uranium.CodeAnalysis.Symbols
{
    public sealed class VariableSymbol : Symbol
    {
        internal VariableSymbol(string name, bool isReadOnly, TypeSymbol type, SyntaxToken? identifierToken)
            : base(name)
        {
            IsReadOnly = isReadOnly;
            Type = type;
            IdentifierToken = identifierToken;
        }

        public bool IsReadOnly { get; }
        public TypeSymbol Type { get; }
        public SyntaxToken? IdentifierToken { get; }

        public override SymbolKind Kind => SymbolKind.Variable; 
    }
}
