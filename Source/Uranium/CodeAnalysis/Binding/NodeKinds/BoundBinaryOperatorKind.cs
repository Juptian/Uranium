﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Binding.NodeKinds
{
    //Hey guess what, it's yet another enum!
    internal enum BoundBinaryOperatorKind
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Pow,

        AdditionEquals,
        SubtractionEquals,
        MultiplicationEquals,
        DivisionEquals,
        
        AdditionAddition,
        SubtractionSubtraction,

        BitwiseAND,
        BitwiseOR,
        BitwiseXOR,

        LogicalAND,
        LogicalOR,
        LogicalXOREquals,
        LogicalXOR,
        LogicalEquals,
        NotEquals,
        LesserThan,
        LesserThanEquals,
        GreaterThan,
        GreaterThanEquals,
    }
}
