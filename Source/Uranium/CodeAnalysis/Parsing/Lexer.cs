using System;
using System.Diagnostics;
using System.Linq;
using Uranium.Logging;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Text;
using Uranium.CodeAnalysis.Lexing.LexerSupport;

namespace Uranium.CodeAnalysis.Lexing
{
    internal sealed class Lexer
    {
        public char PreviousIndex => Peek(-1);
        public char CurrentIndex => Peek(0);
        public char NextIndex => Peek(1);

        public int Start;
        public int Index;
        
        public string? Text;
        public object? CurrentValue;
        
        public SyntaxKind Current;

        public SyntaxToken? PreviousIdentifier;
        public readonly SourceText Source;
        public readonly DiagnosticBag diagnostics = new();

        public Lexer(SourceText contents) => Source = contents;
        public DiagnosticBag Diagnostics => diagnostics;

        public SyntaxToken Lex()
        {
            CurrentValue = null;
            Current = SyntaxKind.BadToken;
            ReadSpecialChars(false);
            LexToken(CurrentIndex);

            var token = new SyntaxToken(Current, Index++, Text ?? PreviousIndex.ToString(), CurrentValue);
            if (SyntaxFacts.GetKeywordType(Current) is not null)
            {
                PreviousIdentifier = token;
            }
            return token;
        }

        public bool Match(char ch, int offset)
        {
            if (ch.Equals(Peek(offset)))
            {
                Index++;
                return true;
            }
            return false;
        }

        private char Peek(int offset)
        {
            var peekIndex = Index + offset;
            return peekIndex >= Source.Length ? '\0' : Source[peekIndex];    
        }

        private void ReadSpecialChars(bool keepGoing)
        {
            var finished = false;
            var Start = Index;
            while (!finished)
            {
                switch (CurrentIndex)
                {
                    //Special characters
                    case '\r':
                    case '\n':
                        Current = SyntaxKind.LineBreak;
                        WhitespaceReader.ReadLineBreak(this);
                        finished = !keepGoing;
                        Text = finished ? "\n" : Text;
                        break;
                    case ' ':
                    case '\t':
                        Index++;
                        WhitespaceReader.ReadWhitespace(this);
                        Current = SyntaxKind.WhiteSpace;
                        break;
                    case '\0':
                        Current = SyntaxKind.EndOfFile;
                        finished = true;
                        break;
                    default:
                        if (char.IsWhiteSpace(CurrentIndex))
                        {
                            Index++;
                            WhitespaceReader.ReadWhitespace(this);
                            Current = SyntaxKind.WhiteSpace;
                        }
                        else
                        {
                            finished = true;
                        }
                        break;
                }
                var length = Index - Start;
                if (length > 0)
                {
                    Text  = Source.ToString(Start, length);
                }
            }
        }

        //Literally lexes a single token
        //Yes this lexes keywords
        public void LexToken(char ch)
        {
            Start = Index;
            //Using a bool to check conditions with more ease
            switch (ch)
            {
                //Math operators
                case '+':
                    OperatorSigns.PlusSign(this);
                    break;
                case '-':
                    OperatorSigns.MinusSign(this);
                    break;
                case '*':
                    OperatorSigns.MultiplySign(this);
                    break;
                case '/':
                    OperatorSigns.DivideSign(this);
                    break;
                case '>':
                    Current = Match('=', 1) ? SyntaxKind.GreaterThanEquals : SyntaxKind.GreaterThan;
                    break;
                case '<':
                    Current = Match('=', 1) ? SyntaxKind.LesserThanEquals : SyntaxKind.LesserThan;
                    break;
                case '=':
                    Current = Match('=', 1) ? SyntaxKind.DoubleEquals : SyntaxKind.Equals;
                    break;
                case '^':
                    Current = Match('=', 1) ? SyntaxKind.HatEquals : SyntaxKind.Hat;
                    break;
                case '!':
                    Current = Match('=', 1) ? SyntaxKind.BangEquals : SyntaxKind.Bang;
                    break;

                //Also operators
                case '|':
                    Current = Match('|', 1) ? SyntaxKind.DoublePipe : SyntaxKind.Pipe;
                    break;
                case '&':
                    Current = Match('&', 1)  ? SyntaxKind.DoubleAmpersand : SyntaxKind.Ampersand;
                    break;
                case '%':
                    Current = Match('=', 1) ? SyntaxKind.PercentEquals : SyntaxKind.Percent;
                    break;
                case '~':
                    Current = SyntaxKind.Tilde;
                    break;

                //Numbers
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    LexNumbers.ReadNum(this);
                    Current = SyntaxKind.NumberToken;
                    break;

                //Pure syntax
                case ';':
                    Current = SyntaxKind.Semicolon;
                    break;
                case '(':
                    Current = SyntaxKind.OpenParenthesis;
                    break;
                case ')':
                    Current = SyntaxKind.CloseParenthesis;
                    break;
                case '{':
                    Current = SyntaxKind.OpenCurlyBrace;
                    break;
                case '}':
                    Current = SyntaxKind.CloseCurlyBrace;
                    break;
                case '[':
                    Current = SyntaxKind.OpenBrackets;
                    break;
                case ']':
                    Current = SyntaxKind.CloseBrackets;
                    break;
                case ',':
                    Current = SyntaxKind.Comma;
                    break;
                case '.':
                    Current = SyntaxKind.Dot;
                    break;
                case ':':
                    Current = SyntaxKind.Colon;
                    break;

                case '#':
                    if(Match('"', 1))
                    {
                        
                        StringLexer.ReadString(this, true, false);
                    }
                    else if(Match('$',1) && Match('"', 2))
                    {
                        Index += 2;
                        StringLexer.ReadString(this, true, true);
                    }
                    break;
                case '"':
                    StringLexer.ReadString(this, false, false);
                    return;
                
                case '\0':
                    Current = SyntaxKind.EndOfFile;
                    return;
                //If it matches none of the above, we want to check if it's a character.
                //This way our identifiers and keywords still work
                default:
                    if (char.IsLetter(ch))
                    {
                        ReadIdentifierOrKeyword();
                    }
                    break;
            }
            GetText();
        }

        private void ReadIdentifierOrKeyword()
        {
            var Start = Index;
            while (char.IsLetter(CurrentIndex))
            {
                Index++;
            }
            var length = Index - Start;
            var text = Source.ToString(Start, length);
            var kind = SyntaxFacts.GetKind(text);
            
            Current = kind == SyntaxKind.BadToken ? SyntaxKind.IdentifierToken : kind;
            Text = text;
            Index--;
        }

        private void GetText()
        {
            //Checking here for if it's an identifier token, this way we don't do anything with it.
            //This is because Identifier tokens should not be modified.
            if(!SyntaxFacts.GetText(Current).Equals("BadToken", StringComparison.OrdinalIgnoreCase))
            {
                var length = Index - Start;
                var text = SyntaxFacts.GetText(Current);

                //If I remove the "text is null"
                //The tests fucking die
                //So we leave it here.
                if ((text is null && Text is null) || text!.Equals("BadToken", StringComparison.OrdinalIgnoreCase))
                {
                    text = Source.ToString(Start, length);
                }
                Text = text;
            }
        }

    }
}
