using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Text
{
    public sealed class TextLine
    {
        public TextLine(SourceText text, int start, int length, int lengthIncludingLineBreak)
        {
            Text = text;
            Start = start;
            Length = length;
            LengthIncludingLineBreak = lengthIncludingLineBreak;
        }

        public SourceText Text { get; }
        public int Start { get; }
        public int Length { get; }
        public int LengthIncludingLineBreak { get; }

        public int End => Start + Length;
        public TextSpan Span => new(Start, Length);
        public TextSpan SpanWithLineBreak => new(Start, LengthIncludingLineBreak);
        public override string ToString() => Text.ToString(Span);

    }
}
