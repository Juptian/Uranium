using System;
using System.Collections.Generic;
using Uranium.CodeAnalysis.Binding;
using Uranium.CodeAnalysis.Binding.Statements;
using Uranium.CodeAnalysis.Binding.NodeKinds;
using Uranium.CodeAnalysis.Text;

namespace Uranium.CodeAnalysis.Syntax.Expression
{
    //Basically just the teacher that only gives
    //pop quizzes and tests
    //It just evaluates
    internal sealed class Evaluator
    {
        private readonly BoundStatement _root;
        private readonly Dictionary<VariableSymbol, object> _variables;

        private object? _lastValue;

        public Evaluator(BoundStatement root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
        {
            EvaluateStatement(_root);
            return _lastValue!;
        }

        private object EvaluateExpression(BoundExpression node)
        {
            switch(node)
            {
                //if it's a literal expression, return it's value
                case BoundLiteralExpression n: 
                    return n.Value;

                //if it's a Unary expression, we just evaluate the operand
                //and return it's value according to the symbol
                //Moved into it's own function because it's kinda chonky
                case BoundUnaryExpression u:
                    return EvaluateBoundUnaryExpression(u);

                case BoundVariableExpression v:
                    return _variables[v.Variable];

                case BoundAssignmentExpression a:
                    var value = EvaluateExpression(a.Expression);
                    _variables[a.Variable] = value;
                    return value;

                //If it's none of the above, we check out last resort
                //A BoundBinaryExpression, here we evaluate the left and right sides of both expressions
                //then return a value based off of the current operator kind
                //Also moved into it's own function because it's pretty fat
                case BoundBinaryExpression b:
                    return EvaluateBoundBinaryExpression(b);
                default:
                    //Same as above ^^
                    throw new($"Unexpected node {node.Kind}");
            }
            //Same as above ^^
            throw new($"Unexpected node {node.Kind}");
        }

        private void EvaluateStatement(BoundStatement statement)
        {
            switch(statement.Kind)
            {
                case BoundNodeKind.BlockStatement:
                    EvaluateBlockStatement( (BoundBlockStatement)statement);
                    return;

                case BoundNodeKind.ExpressionStatement:
                    EvaluateExpressionStatement( (BoundExpressionStatement)statement);
                    return;
                case BoundNodeKind.VariableDeclaration:
                    EvaluateVariableDeclaration( (BoundVariableDeclaration)statement);
                    return;

                case BoundNodeKind.IfStatement:
                    EvaluateIfStatement( (BoundIfStatement)statement );
                    return;

                case BoundNodeKind.WhileStatement:
                    EvaluateWhileStatement( (BoundWhileStatement)statement );
                    return;

                default:
                    //Exhausted all our options, time to call it quits!
                    throw new($"Unexpected statement {statement}");
            }
        }


        private void EvaluateBlockStatement(BoundBlockStatement statement)
        {
            foreach(var item in statement.Statements)
            {
                EvaluateStatement(item);
            }
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement statement) => _lastValue = EvaluateExpression(statement.Expression); 

        private void EvaluateVariableDeclaration(BoundVariableDeclaration statement)
        {
            var value = EvaluateExpression(statement.Initializer);
            _variables[statement.Variable] = value;
            _lastValue = value;
        }

        private void EvaluateIfStatement(BoundIfStatement statement)
        {
            if( (bool)EvaluateExpression(statement.Condition) )
            {
                EvaluateStatement(statement.Statement);
            }
            else if(statement.ElseStatement is not null)
            {
                EvaluateStatement(statement.ElseStatement);
            }
        }

        private void EvaluateWhileStatement(BoundWhileStatement statement)
        {
            while( (bool)EvaluateExpression(statement.Condition))
            {
                EvaluateStatement(statement.Body);
            }
        }

        private object EvaluateBoundUnaryExpression(BoundUnaryExpression u)
        {
            var operand = EvaluateExpression(u.Operand);
            switch(u.Op.Kind)
            {
                case BoundUnaryOperatorKind.Identity:
                    return (int)operand;
                case BoundUnaryOperatorKind.Negation:
                    return -(int)operand;
                case BoundUnaryOperatorKind.LogicalNegation:
                    return !(bool)operand;
            }
            Console.Error.WriteLine($"Unexpected unary operator {u.Kind}");
            return new object();
        }

        private object EvaluateBoundBinaryExpression(BoundBinaryExpression b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);
            switch (b.Op.Kind)  
            {
                //Universal
                case BoundBinaryOperatorKind.LogicalEquals:
                    return LeftEqualsRight(left, right); 
                case BoundBinaryOperatorKind.NotEquals:
                    return !LeftEqualsRight(left, right);

                //Int
                case BoundBinaryOperatorKind.Addition:
                    return (int)left + (int)right;
                case BoundBinaryOperatorKind.Subtraction:
                    return (int)left - (int)right;
                case BoundBinaryOperatorKind.Multiplication:
                    return (int)left * (int)right;
                case BoundBinaryOperatorKind.Division:
                    return (int)left / (int)right;

                case BoundBinaryOperatorKind.LesserThan:
                    return (int)left < (int)right;
                case BoundBinaryOperatorKind.LesserThanEquals:
                    return (int)left <= (int)right;
                case BoundBinaryOperatorKind.GreaterThan:
                    return (int)left > (int)right;
                case BoundBinaryOperatorKind.GreaterThanEquals:
                    return (int)left >= (int)right;


                //C# doesn't like casting a double ot an int, so I had to work around it...
                //Which I decided was to make my own recursive Pow function
                case BoundBinaryOperatorKind.Pow:
                    return Pow( (int) left, (int) right);
                
                //Bool
                case BoundBinaryOperatorKind.LogicalAND:
                    return (bool)left && (bool)right;
                case BoundBinaryOperatorKind.LogicalOR:
                    return (bool)left || (bool)right;
                case BoundBinaryOperatorKind.LogicalXOREquals:
                    var leftBool = (bool)left;
                    var rightBool = (bool)right;
                    return leftBool ^= rightBool;
                case BoundBinaryOperatorKind.LogicalXOR:
                    return (bool)left ^ (bool)right;
                    
                default:
                    //We can throw exceptions here because we've exhausted all options,
                    //and this is an internal Uranium error, should handle this more gracefully,
                    //but during the development stage, and exception will provide more info,
                    //on the stack trace :)
                    throw new($"Unexpected binary operator {b.Op.Kind}");
            }
        }

        private static bool LeftEqualsRight(object left, object right)
        {
            if(left.GetType() == typeof(int) && right.GetType() == typeof(bool))
            {
                if((int)left == 0)
                {
                    return (bool)right == false;
                }
                else
                {
                    return (bool)right == true;
                }
            }
            else if(left.GetType() == typeof(bool) && right.GetType() == typeof(int))
            {
                if((int)right == 0)
                {
                    return (bool)left == false;
                }
                else
                {
                    return (bool)left == true;
                }

            }
            return Equals(left, right);
        }

        private static int Pow(int number, int power)
        {
            if(power == 1)
            {
                return number;
            }
            //Another default case in case someone tries to be funny
            else if(power <= 0) 
            {
                return 0;
            }
            return number * Pow(number, --power);
        }
    }
}
