using System;
using System.Collections.Immutable;

namespace Uranium.CodeAnalysis.Text
{
    public sealed class SourceText
    {
        private readonly string _text;

        private SourceText(string text)
        {
            _text = text;
            Lines = ParseLines(this, text);
        }
        public ImmutableArray<TextLine> Lines { get; }

        public char this[int index] => _text[index];

        public int Length => _text.Length;

        public int GetLineIndex(int position)
        {
            var lower = 0;
            var upper = Lines.Length - 1;

            //Binary search for Ө(log n) time
            //As well as O(1) time.
                //So it scales linearly.
            //And Ω(1)
                //So the best case is that it finds it instantly
            //Aka fast boi
            while(lower <= upper)
            {
                var index = lower + (upper - lower) / 2;
                var start = Lines[index].Start;

                if(start == position)
                {
                    return index;
                }
                else if(start > position)
                {
                    upper = index - 1;
                } 
                else
                {
                    lower = index + 1;
                }
            }
            return lower - 1;
        }

        private static ImmutableArray<TextLine> ParseLines(SourceText sourceText, string text)
        {
            var result = ImmutableArray.CreateBuilder<TextLine>();

            var lineStart = 0;
            var position = 0;

            while (position < text.Length)
            {
                var lineBreakWidth = GetLineBreakWidth(text, position);

                if (lineBreakWidth == 0)
                {
                    position++;
                }
                else
                {
                    AddLine(result, sourceText, position, lineStart, lineBreakWidth);
                    position += lineBreakWidth;
                    lineStart = position;
                }
            }

            if (position > lineStart)
            {
                AddLine(result, sourceText, position, lineStart, 0);
            }

            return result.ToImmutable();
        }

        private static void AddLine(ImmutableArray<TextLine>.Builder result, SourceText sourceText, int position, int lineStart, int lineBreakWidth)
        {
            var lineLength = position - lineStart;
            var lineLengthWithLineBreak = lineLength + lineBreakWidth;
            var line = new TextLine(sourceText, lineStart, lineLength, lineLengthWithLineBreak);
            result.Add(line);
        }

        private static int GetLineBreakWidth(string text, int i)
        {
            var current = text[i];
            var lookAhead = i + 1 >= text.Length ? '\0' : text[i + 1];

            if(current.Equals('\r') && lookAhead.Equals('\n')) { return 2; }
            else if(current.Equals('\r') || current.Equals('\n')) { return 1; }
            return 0;
        }

        public static SourceText From(string text)
        {
            return new(text);
        }

        public override string ToString() => _text;

        public string ToString(int start, int length) => _text.Substring(start, length);

        public string ToString(TextSpan span) => _text.Substring(span.Start, span.Length);
    }
}
