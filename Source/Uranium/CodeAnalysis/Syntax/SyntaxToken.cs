using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Lexing;
using Uranium.CodeAnalysis.Text;

namespace Uranium.CodeAnalysis.Syntax
{
    public class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxKind kind, int position, string? text, object? value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }
        public override SyntaxKind Kind { get; }
        public int Position { get; }
        public string? Text { get; }
        public object? Value { get; }

        public override TextSpan Span => new(Position, Text?.Length ?? 0);

        public override string ToString()
        {
            return $"{Kind}, {Position}, {Text}";
        }
    }
}