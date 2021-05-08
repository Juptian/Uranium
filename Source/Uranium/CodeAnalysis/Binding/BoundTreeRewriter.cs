using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Binding.Statements;
using Uranium.CodeAnalysis.Binding.NodeKinds;

namespace Uranium.CodeAnalysis.Binding
{
    internal abstract class BoundTreeRewriter
    {
        /*
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        VariableExpression,
        AssignmentExpression,

        
        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        IfStatement,
        ElseStatement,
        WhileStatement,
        ForStatement,*/

        public virtual BoundStatement RewriteStatement(BoundStatement node)
        {
            return node.Kind switch
            {
                BoundNodeKind.BlockStatement => RewriteBlockStatement( (BoundBlockStatement)node),
                BoundNodeKind.ExpressionStatement => RewriteExpressionStatement( (BoundExpressionStatement)node),
                BoundNodeKind.VariableDeclaration => RewriteVariableDeclaration( (BoundVariableDeclaration)node),
                BoundNodeKind.IfStatement => RewriteIfStatement( (BoundIfStatement) node),
                BoundNodeKind.WhileStatement => RewriteWhileStatement( (BoundWhileStatement)node),
                BoundNodeKind.ForStatement => RewriteForStatement( (BoundForStatement)node),
                _ => throw new($"Unexpected node: {node.Kind}"),
            };
        }

        public virtual BoundExpression RewriteExpression(BoundExpression node)
        {

            return node.Kind switch
            {
                BoundNodeKind.UnaryExpression => RewriteUnaryExpression((BoundUnaryExpression)node),
                BoundNodeKind.LiteralExpression => RewriteLiteralExpression((BoundLiteralExpression)node),
                BoundNodeKind.BinaryExpression => RewriteBinaryExpression((BoundBinaryExpression)node),
                BoundNodeKind.VariableExpression => RewriteVariableExpression((BoundVariableExpression)node),
                BoundNodeKind.AssignmentExpression => RewriteAssignmentExpression((BoundAssignmentExpression)node),
                _ => throw new($"Unexpected node: {node.Kind}"),
            };
        }

        protected virtual BoundStatement RewriteBlockStatement(BoundBlockStatement node)
        {
            ImmutableArray<BoundStatement>.Builder builder = null;
            for(var i = 0; i < node.Statements.Length; i++)
            {
                var oldStatement = node.Statements[i];
                var newStatement = RewriteStatement(oldStatement);
                if(newStatement != oldStatement)
                {
                    if(builder is null)
                    {
                        builder = ImmutableArray.CreateBuilder<BoundStatement>(node.Statements.Length);
                        for(var j = 0; j < i; j++)
                        {
                            builder.Add(node.Statements[j]);
                        }
                    }
                }
                if(builder is not null)
                {
                    builder.Add(newStatement);
                }
            }
            if(builder is null)
            {
                return node;
            }

            return new BoundBlockStatement(builder.MoveToImmutable());

        }

        protected virtual BoundStatement RewriteExpressionStatement(BoundExpressionStatement node)
        {
            var expression = RewriteExpression(node.Expression);
            if(expression == node.Expression)
            {
                return node;
            }
            return new BoundExpressionStatement(expression);
        }

        protected virtual BoundStatement RewriteVariableDeclaration(BoundVariableDeclaration node)
        {
            return node;
        }

        protected virtual BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            return node;
        }

        protected virtual BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            return node;
        }

        protected virtual BoundStatement RewriteForStatement(BoundForStatement node)
        {
            BoundStatement? initializer = null;
            BoundExpression? condition = null;
            BoundExpression? incrementation = null;
            if(node.VariableDeclaration is not null)
            {
                initializer = RewriteStatement(node.VariableDeclaration);
            }

            if(node.Condition is not null)
            {
                condition = RewriteExpression(node.Condition);
            }

            if(node.Increment is not null)
            {
                incrementation = RewriteExpression(node.Increment);
            }

            if(initializer is null && condition is null && incrementation is null)
            {
            }

            return node;
        }


        protected virtual BoundExpression RewriteUnaryExpression(BoundUnaryExpression node)
        {
            var operand = RewriteExpression(node.Operand);
            if(operand == node.Operand)
                return node;
            return new BoundUnaryExpression(node.Op, operand);
        }

        protected virtual BoundExpression RewriteLiteralExpression(BoundLiteralExpression node)
        {
            return node;
        }

        protected virtual BoundExpression RewriteBinaryExpression(BoundBinaryExpression node)
        {
            var left = RewriteExpression(node.Left);
            var right = RewriteExpression(node.Right);
            if(left == node.Left && right == node.Right)
            {
                return node;
            }
            return new BoundBinaryExpression(left, node.Op, right);
        }

        protected virtual BoundExpression RewriteVariableExpression(BoundVariableExpression node)
        {
            return node;
        }

        protected virtual BoundExpression RewriteAssignmentExpression(BoundAssignmentExpression node)
        {
            var expression = RewriteExpression(node.Expression);
            if(expression == node.Expression)
            {
                return node;
            }
            return new BoundAssignmentExpression(node.Variable, expression, node.CompoundOperator, node.IsCompound);
        }

    }
}
