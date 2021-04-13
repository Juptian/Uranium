using System;
using System.Collections.Generic;
using Compiler.Lexing;

namespace Compiler.Syntax
{
    abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}
