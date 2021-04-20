using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Uranium.CodeAnalysis.Text;

namespace Uranium.Tests.CodeAnalysis.Other
{
    internal sealed class AnnotatedText
    {
        public AnnotatedText(string text, ImmutableArray<TextSpan> spans)
        {
            Text = text;
            Spans = spans;
        }

        public string Text { get; }
        public ImmutableArray<TextSpan> Spans { get; }

        public static AnnotatedText Parse(string text)
        {
            text = RemoveIndents(text);

            var textBuilder = new StringBuilder();
            var spanBuilder = ImmutableArray.CreateBuilder<TextSpan>();
            var startStack = new Stack<int>();

            var position = 0;

            foreach(var ch in text)
            {
                if(ch.Equals('['))
                {
                    startStack.Push(position);
                }
                else if (ch.Equals(']'))
                {
                    if (startStack.Count == 0)
                    {
                        throw new ArgumentException("Too many ']'s in text ", nameof(text));
                    }
                    var start = startStack.Pop();
                    var end = position;
                    var span = TextSpan.FromBounds(start, end);
                    spanBuilder.Add(span);
                }
                else
                {
                    position++;
                    textBuilder.Append(ch);
                }
            }

            if(startStack.Count is not 0)
            {
                throw new ArgumentException("Too few ']'s in text ", nameof(text));
            }

            return new(textBuilder.ToString(), spanBuilder.ToImmutable());
        }

        public static string RemoveIndents(string text)
        {
            var lines = new List<string>();
            using var stringReader = new StringReader(text);
            string line;

            while ((line = stringReader.ReadLine()) is not null)
            {
                lines.Add(line);
            }

            stringReader.Dispose();

            var minIndentation = int.MaxValue;
            for(var i = 0; i < lines.Count; i++)
            {
                line = lines[i];
                if(line.Trim().Length == 0)
                {
                    lines[i] = string.Empty;
                    continue;
                }

                var indentation = line.Length - line.TrimStart().Length;
                minIndentation = Math.Min(indentation, minIndentation);
            }

            for(var i = 0; i < lines.Count; i++)
            {
                if(lines[i].Length == 0)
                {
                    continue;
                }
                lines[i] = lines[i][minIndentation..];
            }

            while(lines.Count > 0 && lines[0].Length == 0)
            {
                lines.RemoveAt(0);
            }
            while(lines.Count > 0 && lines[^1].Length == 0)
            {
                lines.RemoveAt(lines.Count - 1);
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}
