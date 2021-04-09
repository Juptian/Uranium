using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Lexer
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

        //Syntaxic symbols
        Semicolon,
        Period,
        OpenParenthasis,
        CloseParenthasis,
        OpenCurlyBrackets,
        CloseCurlyBrackets,
        OpenDiamondBrackets,
        CloseDiamondBrackets,
        TypeDeclarator,
        Null
    }
}
