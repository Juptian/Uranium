namespace Uranium.CodeAnalysis.Syntax
{ 
    public enum SyntaxKind 
    {
        //Math symbols
        Plus,
        Minus,
        Multiply,
        Divide,
        Pow,

        PlusEquals,
        MinusEquals,
        MultiplyEquals,
        DivideEquals,
        PowEquals,

        GreaterThan,
        LesserThan,
        Equals,
        GreaterThanEquals,
        LesserThanEquals,
        DoubleEquals,
        PercentEquals,
        PlusPlus,
        MinusMinus,

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
        IdentifierToken,

        //Logic keywords
        IfKeyword,
        ElseKeyword,
        BreakKeyword,
        ContinueKeyword,
        DefaultKeyword,

        //Specifiers
        ConstKeyword,
        LetConstKeyword,
        ReturnKeyword,
        ThisKeyword,
        ToKeyword,

        //Bool keywords
        TrueKeyword,
        FalseKeyword,

        //Type keywords
        ClassKeyword,
        StructKeyword,
        EnumKeywrod,
        NamespaceKeyword,

        IntKeyword,
        FloatKeyword,
        DoubleKeyword,
        LongKeyword,
        VarKeyword,
        CharKeyword,
        StringKeyword,
        BoolKeyword,
        TypeDefKeyword,

        //Loop keywords
        WhileKeyword,
        DoKeyword,
        ForKeyword,

        // Statements
        BlockStatement,
        BreakStatement,
        ContinueStatement,
        DoWhileStatement,
        ExpressionStatement,
        ForStatement,
        IfStatement,
        ElseStatement,
        MemberBlockStatement,
        ReturnStatement,
        VariableDeclaration,
        WhileStatement,

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

        //Compilation Unit
        CompilationUnit,
        //End of file
        EndOfFile,

        //Token be bad
        BadToken
    }
}
