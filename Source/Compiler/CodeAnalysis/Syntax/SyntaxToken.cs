using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.CodeAnalysis.Lexing;
#pragma warning disable CS8632
namespace Compiler.CodeAnalysis.Syntax
{
    internal class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }
        public override SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object? Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }

        public override string ToString()
        {
            return $"{Kind}, {Position}, {Text}";
        }
    }
}