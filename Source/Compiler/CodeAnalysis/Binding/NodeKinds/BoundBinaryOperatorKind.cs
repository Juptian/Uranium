using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.CodeAnalysis.Binding.NodeKinds
{
    //Hey guess what, it's yet another enum!
    internal enum BoundBinaryOperatorKind
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Pow,
        LogicalAND,
        LogicalOR,
        LogicalXOREquals,
        LogicalXOR,
        LogicalEquals,
        NotEquals
    }
}
