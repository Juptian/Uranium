using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.CodeAnalysis.Binding.NodeKinds
{
    //Yet another enum
   internal enum BoundUnaryOperatorKind
   {
        Identity,
        Negation,
        LogicalNegation,
        LogicalXOR
    } 
}
