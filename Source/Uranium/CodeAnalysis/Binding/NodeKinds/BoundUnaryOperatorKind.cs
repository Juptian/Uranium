using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Binding.NodeKinds
{
    //Yet another enum
   internal enum BoundUnaryOperatorKind
   {
        Identity,
        Negation,
        LogicalNegation,

        BitwiseNegation
    } 
}
