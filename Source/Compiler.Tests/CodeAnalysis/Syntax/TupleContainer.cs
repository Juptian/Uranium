using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.Tests.CodeAnalysis.Syntax
{
    public sealed class TupleContainer
    {
        public TupleContainer(SyntaxKind kind, string text)
        {
            Kind = kind;
            Text = text;
        }

        public SyntaxKind Kind { get; }
        public string Text { get; }
    }
}
