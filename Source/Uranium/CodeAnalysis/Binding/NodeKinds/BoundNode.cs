using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Binding.NodeKinds
{
    //Base class
    //Yep that's it!
    internal abstract class BoundNode
    {
        public abstract BoundNodeKind Kind { get; }
    }
}
