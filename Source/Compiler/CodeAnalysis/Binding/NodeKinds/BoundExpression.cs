using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.CodeAnalysis.Binding.NodeKinds
{
    //Yet another base class
    internal abstract class BoundExpression : BoundNode
    {
        public abstract Type Type { get; }
    }
}
