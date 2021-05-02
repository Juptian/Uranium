using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Expression;
using Uranium.CodeAnalysis.Syntax.Statement;

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

            var expression = ParseExpression(text);

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

            var expression = ParseExpression(text);

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

        [Theory]
        [InlineData(ParserTestCases.CaseOne, SyntaxKind.VarKeyword, "var", "i", "0", SyntaxKind.LesserThanEquals, "10", "1", SyntaxKind.PlusEquals)]
        [InlineData(ParserTestCases.CaseTwo, SyntaxKind.IntKeyword, "int", "a", "0", SyntaxKind.LesserThanEquals, "100", "1", SyntaxKind.PlusEquals)]
        [InlineData(ParserTestCases.CaseThree, SyntaxKind.VarKeyword, "var", "i", "0", SyntaxKind.LesserThanEquals, "1000", "1", SyntaxKind.PlusEquals)]
        [InlineData(ParserTestCases.CaseFour, SyntaxKind.VarKeyword, "var", "i", "0", SyntaxKind.BangEquals, "1000", "1", SyntaxKind.PlusEquals)]
        [InlineData(ParserTestCases.CaseFive, SyntaxKind.FloatKeyword, "float", "f", "0", SyntaxKind.LesserThan, "100", "1", SyntaxKind.PlusEquals)]
        [InlineData(ParserTestCases.CaseSix, SyntaxKind.VarKeyword, "var", "i", "100", SyntaxKind.GreaterThanEquals, "10", "1", SyntaxKind.MinusEquals)]
        [InlineData(ParserTestCases.CaseEight, SyntaxKind.IntKeyword, "int", "a", "0", SyntaxKind.GreaterThanEquals, "100", "1", SyntaxKind.MinusEquals)]
        [InlineData(ParserTestCases.CaseNine, SyntaxKind.VarKeyword, "var", "i", "2000", SyntaxKind.BangEquals, "1000", "1", SyntaxKind.MinusEquals)]
        [InlineData(ParserTestCases.CaseTen, SyntaxKind.VarKeyword, "var", "i", "2000", SyntaxKind.GreaterThanEquals, "1000", "1", SyntaxKind.MinusEquals)]
        [InlineData(ParserTestCases.CaseEleven, SyntaxKind.FloatKeyword, "float", "f", "200", SyntaxKind.GreaterThan, "100", "1", SyntaxKind.MinusEquals)]
        public void ParserParsesForLoops
            (
                string data, 
                SyntaxKind keyword, string keywordName,
                string initializerName, string initializerValue, 
                SyntaxKind conditionToken, string conditionText, 
                string incrementationText, SyntaxKind incrementor
            )
        {
            var soloOperator = SyntaxFacts.GetSoloOperator(incrementor);
            var soloText = SyntaxFacts.GetText(soloOperator);
            var expression = ParseForLoop(data);
            using var e = new AssertingEnumerator(expression);

            Assert.Equal(incrementor, SyntaxFacts.GetKind(SyntaxFacts.GetText(incrementor)));
            //This is indented according to the AST produced from for loops
            e.AssertNode(SyntaxKind.ForStatement);
                e.AssertToken(SyntaxKind.ForKeyword, "for");
                e.AssertToken(SyntaxKind.OpenParenthesis, "(");
                    e.AssertVariableDeclaration(keyword, keywordName, initializerName, initializerValue);
                    e.AssertToken(SyntaxKind.Semicolon, ";");

                    e.AssertCondition(initializerName, conditionToken, conditionText);
                    e.AssertToken(SyntaxKind.Semicolon, ";");

                    e.AssertAssignmentExpression(initializerName, soloOperator, soloText, incrementationText, incrementor);
                e.AssertToken(SyntaxKind.CloseParenthesis, ")");
                e.AssertBlockStatement(true);
        }
        

        private static ExpressionSyntax ParseExpression(string text)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var root = syntaxTree.Root;
            var statement = root.Statement;
            return Assert.IsType<ExpressionStatementSyntax>(statement).Expression;
        }

        private static ForStatementSyntax ParseForLoop(string text)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var root = syntaxTree.Root;
            var statement = root.Statement;
            return Assert.IsType<ForStatementSyntax>(statement);
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
