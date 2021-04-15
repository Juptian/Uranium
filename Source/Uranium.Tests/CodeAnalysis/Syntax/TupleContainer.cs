using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Syntax;

namespace Uranium.Tests.CodeAnalysis.Syntax
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
