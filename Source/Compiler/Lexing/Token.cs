using System;

namespace Compiler.Lexing
{
    public sealed class Token
    {
        public Token(TokenType t, int pos, string text, object val = null)
        {
            Type = t;
            Position = pos;
            Text = text;
            Value = val;
        }
        public TokenType Type { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }

        public override string ToString()
        {
            return "Type: " + Type + ", Position: " + Position + ", text: " + Text;
        }
    }
}
