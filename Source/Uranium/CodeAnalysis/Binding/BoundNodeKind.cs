using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Binding
{
    //Literally an enum, what else would it be 
   internal enum BoundNodeKind
   {
        //Expressions
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        VariableExpression,
        AssignmentExpression,

        //Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        IfStatement,
        ElseStatement,
        WhileStatement,
        ForStatement,

        GotoStatement,
        ConditionalGotoStatement,
        LabelStatement,
    }
}
