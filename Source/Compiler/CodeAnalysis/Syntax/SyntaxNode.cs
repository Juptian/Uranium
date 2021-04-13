using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    internal abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}
