namespace Compiler.Lexing
{
    public enum TokenType
    {
        //Math symbols
        Plus,
        Minus,
        Multiply,
        Divide,
        GreaterThan,
        LesserThan,
        Equals,
        PlusEquals,
        MinusEquals,
        MultiplyEquals,
        DivideEquals,
        GreaterThanEquals,
        LesserThanEquals,
        Pow,
        DoubleEquals,
        PercentEquals,

        //Syntactic symbols
        Semicolon,
        Colon,
        Dot,
        Comma,
        Tilde,
        Percent,
        Hat,
        Ampersand,
        Pipe,
        HatEquals,
        DoubleAmpersand,
        DoublePipe,
        OpenParenthesis,
        CloseParenthesis,
        OpenCurlyBrace,
        CloseCurlyBrace,
        TypeDeclarator,
        OpenBrackets,
        CloseBrackets,
        Null,
        LineBreak,
        WhiteSpace,
        SingleLineComment,
        MultiLineComment,

        //
        Number,

        //Token be bad
        BadToken
    }
}
