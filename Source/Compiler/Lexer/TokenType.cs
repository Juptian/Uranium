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

        //Syntaxic symbols
        Semicolon,
        Period,
        OpenParenthesis,
        CloseParenthesis,
        OpenCurlyBrackets,
        CloseCurlyBrackets,
        OpenDiamondBrackets,
        CloseDiamondBrackets,
        TypeDeclarator,
        Null,

        BadToken
    }
}
