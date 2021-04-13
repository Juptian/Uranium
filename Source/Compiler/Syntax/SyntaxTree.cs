using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Syntax.Expression;

namespace Compiler.Syntax
{
    internal sealed class SyntaxTree
    {
        public SyntaxTree(IEnumerable<string> diagnositcs, ExpressionSyntax root, SyntaxToken endOfFileToken)
        {
            Diagnostics = diagnositcs.ToArray();
            Root = root;
            EndOfFileToken = endOfFileToken;
        }

        public IReadOnlyList<string> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }    }
}
