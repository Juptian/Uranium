namespace Compiler.CodeAnalysis.Syntax
{ 
    public enum SyntaxKind 
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
        Bang,
        BangEquals,

        //Temp
        NumberToken,

        //Logic keywords
        IfKeyword,
        ElseKeyword,
        BreakKeyword,
        ContinueKeyword,
        DefaultKeyword,

        //Specifiers
        ConstKeyword,
        ReturnKeyword,
        ThisKeyword,
        ToKeyword,

        //Bool keywords
        TrueKeyword,
        FalseKeyword,

        //Type keywords
        ClassKeyword,
        StructKeyword,
        NamespaceKeyword,
        IntKeyword,
        FloatKeyword,
        DoubleKeyword,
        LongKeyword,
        VarKeyword,

        //Loop keywords
        WhileKeyword,
        DoKeyword,

        //Statements
        IfStatement,
        BreakStatement,
        ContinueStatement,
        VariableDeclaration,
        ForStatement,
        ExpressionStatement,
        ReturnStatement,
        WhileStatement,
        DoWhileStatement,


        // Expressions
        AssignmentExpression,
        BinaryExpression,
        CallExpression,
        CompoundAssignmentExpression,
        LiteralExpression,
        MemberAccessExpression,
        NameExpression,
        ParenthesizedExpression,
        UnaryExpression,
        NamespaceDeclaration,
        NumberExpression,

        //End of file
        EndOfFile,

        //Token be bad
        BadToken
    }
}
