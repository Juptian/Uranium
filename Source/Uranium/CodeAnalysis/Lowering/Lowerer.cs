using System;
using System.Collections.Generic;
using Uranium.CodeAnalysis.Binding;
using Uranium.CodeAnalysis.Binding.Statements;

namespace Uranium.CodeAnalysis.Lowering
{
    internal sealed class Lowerer : BoundTreeRewriter
    {
        private Lowerer()
        {

        }

        public static BoundStatement Lower(BoundStatement statement)
        {
            var lowerer = new Lowerer();
            return lowerer.RewriteStatement(statement);
        }
    }
}
