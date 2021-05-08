using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Uranium.CodeAnalysis.Binding;
using Uranium.CodeAnalysis.Binding.NodeKinds;
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

        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {
            //for(var <identifier>; <identifier> <conditional operator> <condition>; <incrementation>)
            //
            //{
            //      <variable declaration>
            //      while(condition)
            //      {
            //
            //          <body>
            //          <incrementation>
            //      }
            //}
            //

            BoundVariableDeclaration? variableDeclaration = null;
            BoundBinaryExpression? condition = null;
            var trueExpression = new BoundLiteralExpression(true, typeof(bool));
            BoundAssignmentExpression? incrementation = null;
            var body = RewriteStatement(node.Body);

            var whileLoop = new BoundWhileStatement(trueExpression, body);
            
            if(node.VariableDeclaration is not null)
            {
                variableDeclaration = (BoundVariableDeclaration)RewriteStatement(node.VariableDeclaration);
            }
            if(node.Condition is not null)
            {
                condition = (BoundBinaryExpression)RewriteExpression(node.Condition);
            }
            if(node.Increment is not null)
            {
                incrementation = (BoundAssignmentExpression)RewriteExpression(node.Increment);
            }
            
            if(variableDeclaration is null &&
               condition is null &&
               incrementation is null)
            {
                return whileLoop;
            }

            var builder = ImmutableArray.CreateBuilder<BoundStatement>();
            
            if(variableDeclaration is not null) builder.Add(variableDeclaration);

            if(incrementation is not null)
            {
                var incrementAsStatement = new BoundExpressionStatement(incrementation);
                body = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(node.Body, incrementAsStatement));
            }

            builder.Add(condition is null ? whileLoop : new BoundWhileStatement(condition, body));
            

            return new BoundBlockStatement(builder.ToImmutable());
        }
    }
}
