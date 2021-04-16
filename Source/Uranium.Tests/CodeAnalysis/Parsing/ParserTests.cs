using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Uranium.CodeAnalysis.Syntax;

namespace Uranium.Tests.CodeAnalysis.Parsing
{
    public class ParserTests
    {
        [Theory]
        [MemberData(nameof(GetBinaryOperatorPairsData))]
        public void ParserBinaryExpresionFollowsPrecedences(SyntaxKind op1, SyntaxKind op2)
        {
            var op1Precedence = SyntaxFacts.GetBinaryOperatorPrecedence(op1);
            var op2Precedence = SyntaxFacts.GetBinaryOperatorPrecedence(op2);
            var op1Text = SyntaxFacts.GetText(op1);
            var op2Text = SyntaxFacts.GetText(op2);
            var text = $"a {op1Text} b {op2Text} c";

            var expression = SyntaxTree.Parse(text).Root;

            if(op1Precedence >= op2Precedence)
            {
                //      op2
                //     /   \
                //   op1    c  
                //  /   \
                // a     b
                using var e = new AssertingEnumerator(expression);
                
                //indented to visualize with more ease
                //Op2
                e.AssertNode(SyntaxKind.BinaryExpression);
                    //Op1 branch
                    e.AssertNode(SyntaxKind.BinaryExpression);
                        //Op1 - A
                        e.AssertNode(SyntaxKind.NameExpression);
                            e.AssertToken(SyntaxKind.IdentifierToken, "a");
                        //Op1 - Operator
                        e.AssertToken(op1, op1Text);
                            //Op1 - b
                        e.AssertNode(SyntaxKind.NameExpression);
                            e.AssertToken(SyntaxKind.IdentifierToken, "b");
                    //Op2 - Operator
                    e.AssertToken(op2, op2Text);
                    //Op2 - C
                    e.AssertNode(SyntaxKind.NameExpression);
                        e.AssertToken(SyntaxKind.IdentifierToken, "c");
            } 
            else
            {
                //      op1
                //     /   \
                //    a    op2
                //        /   \
                //       b     c
                using var e = new AssertingEnumerator(expression);
                
                //indented to visualize with more ease.
                //Op2
                e.AssertNode(SyntaxKind.BinaryExpression);
                    //Op1 - a
                    e.AssertNode(SyntaxKind.NameExpression);
                        e.AssertToken(SyntaxKind.IdentifierToken, "a");
                    //Op1 - Operator
                    e.AssertToken(op1, op1Text);
                    //Op2
                    e.AssertNode(SyntaxKind.BinaryExpression);
                        //Op2 - B
                        e.AssertNode(SyntaxKind.NameExpression);
                            e.AssertToken(SyntaxKind.IdentifierToken, "b");
                        //Op2 - Operator
                        e.AssertToken(op2, op2Text);
                        //Op2 - C
                        e.AssertNode(SyntaxKind.NameExpression);
                            e.AssertToken(SyntaxKind.IdentifierToken, "c");
            }

        }

        [Theory]
        [MemberData(nameof(GetUnaryOperatorPairsData))]
        public void ParserUnaryExpresionFollowsPrecedences(SyntaxKind unaryKind, SyntaxKind binarykind)
        {
            var unaryPrecedence = SyntaxFacts.GetUnaryOperatorPrecedence(unaryKind);
            var binaryPrecedence = SyntaxFacts.GetBinaryOperatorPrecedence(binarykind);
            var unaryText = SyntaxFacts.GetText(unaryKind);
            var binaryText = SyntaxFacts.GetText(binarykind);
            var text = $"{unaryText} a {binaryText} b";

            var expression = SyntaxTree.Parse(text).Root;

            if (unaryPrecedence >= binaryPrecedence)
            {
                //    bin
                //   /   \ 
                // una    b    
                //  |   
                //  a  
                using var e = new AssertingEnumerator(expression);
                
                //indented to visualize with more ease.
                //Binary 
                e.AssertNode(SyntaxKind.BinaryExpression);
                    //Unary - operatpr
                    e.AssertNode(SyntaxKind.UnaryExpression);
                        //Unary - Operator
                        e.AssertToken(unaryKind, unaryText);
                            //Unary - A
                            e.AssertNode(SyntaxKind.NameExpression);
                                e.AssertToken(SyntaxKind.IdentifierToken, "a");
                    //Binary - Operator
                    e.AssertToken(binarykind, binaryText);
                        //Binary - B
                        e.AssertNode(SyntaxKind.NameExpression);
                            e.AssertToken(SyntaxKind.IdentifierToken, "b");
            }
            else
            {
                //   una
                //    |   
                //   bin      
                //  /   \
                // a     b
                using var e = new AssertingEnumerator(expression);
                
                //Indented to visualize with more ease.
                e.AssertNode(SyntaxKind.UnaryExpression);
                //Unary - Operator
                    e.AssertToken(unaryKind, unaryText);
                    //Binary branch
                    e.AssertNode(SyntaxKind.BinaryExpression);
                        //Binary - A
                        e.AssertNode(SyntaxKind.NameExpression);
                            e.AssertToken(SyntaxKind.IdentifierToken, "a");
                        //Binary - Operator
                        e.AssertToken(binarykind, binaryText);
                        //Binary - b
                        e.AssertNode(SyntaxKind.NameExpression);
                            e.AssertToken(SyntaxKind.IdentifierToken, "b");
            }
        }

        public static IEnumerable<object[]> GetBinaryOperatorPairsData()
        {
            IEnumerable<SyntaxKind> operators = SyntaxFacts.GetBinaryOperators();
            foreach(var op1 in operators)
            {
                foreach(var op2 in operators)
                {
                    yield return new object[] { op1, op2 };
                }
            }
        }

        public static IEnumerable<object[]> GetUnaryOperatorPairsData()
        {
            foreach(var unary in SyntaxFacts.GetUnaryOperators())
            {
                foreach(var binary in SyntaxFacts.GetBinaryOperators())
                {
                    yield return new object[] { unary, binary };
                }
            }
        }
    }
}
