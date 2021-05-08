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
            
            if( (op1 == SyntaxKind.Pow && op2 == SyntaxKind.Pow) || op1Precedence < op2Precedence)
            {
                using var e = new AssertingEnumerator(expression);

                //   op1
                //  /   \
                // a    op2
                //     /   \
                //    b     c
                e.AssertNode(SyntaxKind.BinaryExpression);
                    e.AssertNode(SyntaxKind.NameExpression);
                        e.AssertToken(SyntaxKind.IdentifierToken, "a");
                    e.AssertToken(op1, op1Text);
                    e.AssertNode(SyntaxKind.BinaryExpression);
                        e.AssertNode(SyntaxKind.NameExpression);
                            e.AssertToken(SyntaxKind.IdentifierToken, "b");
                        e.AssertToken(op2, op2Text);
                        e.AssertNode(SyntaxKind.NameExpression);
                            e.AssertToken(SyntaxKind.IdentifierToken, "c");
            }
            else if(op1Precedence >= op2Precedence)
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
        }

        [Theory]
        [MemberData(nameof(GetUnaryOperatorPairsData))]
        public void ParserUnaryExpresionFollowsPrecedences(SyntaxKind unaryKind, SyntaxKind binarykind)
        {
            var unaryPrecedence = SyntaxFacts.GetUnaryOperatorPrecedence(unaryKind);
            var binaryPrecedence = SyntaxFacts.GetBinaryOperatorPrecedence(binarykind);
            var unaryText = SyntaxFacts.GetText(unaryKind);
            var binaryText = SyntaxFacts.GetText(binarykind);
            var text = $"{unaryText}a {binaryText} b";

            var expression = ParseExpression(text);

            if (unaryPrecedence >= binaryPrecedence && binarykind != SyntaxKind.Pow)
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
        [InlineData(ForLoopTestCases.CaseOne, SyntaxKind.VarKeyword, "var", "i", "0", SyntaxKind.LesserThanEquals, "10", "1", SyntaxKind.PlusEquals)]
        [InlineData(ForLoopTestCases.CaseTwo, SyntaxKind.IntKeyword, "int", "a", "0", SyntaxKind.LesserThanEquals, "100", "1", SyntaxKind.PlusEquals)]
        [InlineData(ForLoopTestCases.CaseThree, SyntaxKind.VarKeyword, "var", "i", "0", SyntaxKind.LesserThanEquals, "1000", "1", SyntaxKind.PlusEquals)]
        [InlineData(ForLoopTestCases.CaseFour, SyntaxKind.VarKeyword, "var", "i", "0", SyntaxKind.BangEquals, "1000", "1", SyntaxKind.PlusEquals)]
        [InlineData(ForLoopTestCases.CaseFive, SyntaxKind.FloatKeyword, "float", "f", "0", SyntaxKind.LesserThan, "100", "1", SyntaxKind.PlusEquals)]
        [InlineData(ForLoopTestCases.CaseSix, SyntaxKind.VarKeyword, "var", "i", "100", SyntaxKind.GreaterThanEquals, "10", "1", SyntaxKind.MinusEquals)]
        [InlineData(ForLoopTestCases.CaseEight, SyntaxKind.IntKeyword, "int", "a", "0", SyntaxKind.GreaterThanEquals, "100", "1", SyntaxKind.MinusEquals)]
        [InlineData(ForLoopTestCases.CaseNine, SyntaxKind.VarKeyword, "var", "i", "2000", SyntaxKind.BangEquals, "1000", "1", SyntaxKind.MinusEquals)]
        [InlineData(ForLoopTestCases.CaseTen, SyntaxKind.VarKeyword, "var", "i", "2000", SyntaxKind.GreaterThanEquals, "1000", "1", SyntaxKind.MinusEquals)]
        [InlineData(ForLoopTestCases.CaseEleven, SyntaxKind.FloatKeyword, "float", "f", "200", SyntaxKind.GreaterThan, "100", "1", SyntaxKind.MinusEquals)]
        [MemberData(nameof(GetForLoopData))]
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
            expression.ToString();
        }
        
        [Theory]
        [InlineData(ParserTestCases.CaseOne, SyntaxKind.IntKeyword, "i", "10", SyntaxKind.Plus, "1")]
        [InlineData(ParserTestCases.CaseTwo, SyntaxKind.DoubleKeyword, "d", "10.10", SyntaxKind.Plus, "20.20")]
        [InlineData(ParserTestCases.CaseThree, SyntaxKind.FloatKeyword, "f", "10.3", SyntaxKind.Plus, "10.3")]
        [InlineData(ParserTestCases.CaseFour, SyntaxKind.LongKeyword, "l", "15", SyntaxKind.Plus, "10")]
        [InlineData(ParserTestCases.CaseFive, SyntaxKind.IntKeyword, "i", "10", SyntaxKind.Minus, "1")]
        [InlineData(ParserTestCases.CaseSix, SyntaxKind.DoubleKeyword, "d", "10.1001", SyntaxKind.Minus, "10")]
        [InlineData(ParserTestCases.CaseSeven, SyntaxKind.FloatKeyword, "f", "10.3", SyntaxKind.Minus, "10.3")]
        [InlineData(ParserTestCases.CaseEight, SyntaxKind.LongKeyword, "l", "15", SyntaxKind.Minus, "10")]
        [InlineData(ParserTestCases.CaseNine, SyntaxKind.IntKeyword, "i", "10", SyntaxKind.Pow, "2")]
        [InlineData(ParserTestCases.CaseTen, SyntaxKind.LongKeyword, "l", "100", SyntaxKind.Pow, "2")]
        [InlineData(ParserTestCases.CaseEleven, SyntaxKind.DoubleKeyword, "d", "10", SyntaxKind.Divide, "100")]

        public void ParserParsesVariableDeclaration
            (
                string data, 
                SyntaxKind keyword, string identifier, 
                string valueLeft, SyntaxKind op, string valueRight
            )
        {
            var expression = ParseVariableDeclaration(data);
            using var e = new AssertingEnumerator(expression);
            e.AssertNode(SyntaxKind.VariableDeclaration);
            e.AssertToken(keyword, SyntaxFacts.GetText(keyword));
            e.AssertToken(SyntaxKind.IdentifierToken, identifier);
            e.AssertToken(SyntaxKind.Equals, "=");
            e.AssertNode(SyntaxKind.BinaryExpression);
            e.AssertLiteralExpression();
            e.AssertToken(SyntaxKind.NumberToken, valueLeft);
            e.AssertToken(op, SyntaxFacts.GetText(op));
            e.AssertLiteralExpression();
            e.AssertToken(SyntaxKind.NumberToken, valueRight);
        }

        [Theory]
        [InlineData(WhileLoopTestCases.CaseOne, "x", SyntaxKind.LesserThan, "10")]
        [InlineData(WhileLoopTestCases.CaseTwo, "x", SyntaxKind.LesserThan, "a", false, false)]
        [InlineData(WhileLoopTestCases.CaseThree, "x", SyntaxKind.GreaterThan, "a", false, false)]
        [InlineData(WhileLoopTestCases.CaseFour, "10", SyntaxKind.GreaterThanEquals, "b", true, false)]
        [InlineData(WhileLoopTestCases.CaseFive, "a", SyntaxKind.DoubleEquals, "a", false, false)]
        [InlineData(WhileLoopTestCases.CaseSix, "0", SyntaxKind.LesserThan, "10", true)]
        [InlineData(WhileLoopTestCases.CaseSeven, "x", SyntaxKind.BangEquals, "y", false, false)]
        [MemberData(nameof(GetWhileLoopData))]
        public void ParserParsesWhileLoops
            (
                string data,
                string compared, SyntaxKind op, string comparee,
                bool comparedLiteral = false,
                bool compareeLiteral = true
            )
        {
            var expression = ParseWhileLoop(data);

            using var e = new AssertingEnumerator(expression);
            e.AssertNode(SyntaxKind.WhileStatement);
            e.AssertToken(SyntaxKind.WhileKeyword, "while");
            e.AssertToken(SyntaxKind.OpenParenthesis, "(");
            e.AssertNode(SyntaxKind.BinaryExpression);
            if(comparedLiteral)
            {
                e.AssertLiteralExpression();
                e.AssertToken(SyntaxKind.NumberToken, compared);
            } 
            else
            {
                e.AssertNode(SyntaxKind.NameExpression);
                e.AssertToken(SyntaxKind.IdentifierToken, compared);
            }
            e.AssertToken(op, SyntaxFacts.GetText(op));
            if(compareeLiteral)
            {
                e.AssertLiteralExpression();
                e.AssertToken(SyntaxKind.NumberToken, comparee);
            }
            else
            {
                e.AssertNode(SyntaxKind.NameExpression);
                e.AssertToken(SyntaxKind.IdentifierToken, comparee);
            }
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

        private static WhileStatementSyntax ParseWhileLoop(string text)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var root = syntaxTree.Root;
            var statement = root.Statement;
            return Assert.IsType<WhileStatementSyntax>(statement);
        }

        private static VariableDeclarationSyntax ParseVariableDeclaration(string text)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var root = syntaxTree.Root;
            var statement = root.Statement;
            return Assert.IsType<VariableDeclarationSyntax>(statement);
        }
        
        public static IEnumerable<object[]> GetBinaryOperatorPairsData()
        {
            var operators = SyntaxFacts.GetBinaryOperators().ToArray();
            for(int i = 0; i < operators.Length; i++)
            {
                for(int x = 0; x < operators.Length; x++)
                {
                    yield return new object[] { operators[i], operators[x] };
                }
            }
        }

        public static IEnumerable<object[]> GetUnaryOperatorPairsData()
        {
            var unary = SyntaxFacts.GetUnaryOperators().ToArray();
            var binary = SyntaxFacts.GetBinaryOperators().ToArray();
            for(int i = 0; i < unary.Length; i++)
            {
                for(int x = 0; x < binary.Length; x++)
                {
                    yield return new object[] { unary[i], binary[x] };
                }
            }
        }

        public static IEnumerable<object[]> GetForLoopData()
        {
            var keywords = GetIdentifierKeywords().ToArray();
            for(int i = 0; i < 20; i++)
            {
                for(int x = 0; x < keywords.Length; x++)
                {
                    var text = ForLoopTestCases.MakeForLoop(keywords[x], "i", "0", SyntaxKind.LesserThan, i, SyntaxKind.PlusEquals, "1");
                    yield return new object[] { text, keywords[x], SyntaxFacts.GetText(keywords[x]), "i", "0", SyntaxKind.LesserThan, i.ToString(), "1", SyntaxKind.PlusEquals};
                }
            }
        }

        public static IEnumerable<object[]> GetWhileLoopData()
        {
            var comparisons = GetComparisions().ToArray(); 
            for(int i = 0; i < 20; i++)
            {
                for(int x = 0; x < comparisons.Length; x++)
                {
                    var compOperator = comparisons[x];

                    var text = WhileLoopTestCases.MakeWhileLoop(i, compOperator, "a");
                    yield return new object[] { text, i.ToString(), compOperator, "a", true, false };

                    text = WhileLoopTestCases.MakeWhileLoop("a", compOperator, i);
                    yield return new object[] { text, "a", compOperator, i.ToString() };
                }
            }

            for(int i = 0; i < comparisons.Length; i++)
            {
                    var text = WhileLoopTestCases.MakeWhileLoop("i", comparisons[i], "b");
                    yield return new object[] { text, "i", comparisons[i], "b", false, false };

            }
        }

        public static IEnumerable<SyntaxKind> GetIdentifierKeywords()
        {
            yield return SyntaxKind.VarKeyword;
            yield return SyntaxKind.IntKeyword;
            yield return SyntaxKind.DoubleKeyword;
            yield return SyntaxKind.FloatKeyword;
            yield return SyntaxKind.LongKeyword;
        }

        public static IEnumerable<SyntaxKind> GetComparisions()
        {
            yield return SyntaxKind.LesserThan;
            yield return SyntaxKind.GreaterThan;

            yield return SyntaxKind.LesserThanEquals;
            yield return SyntaxKind.GreaterThanEquals;
            yield return SyntaxKind.BangEquals;
            yield return SyntaxKind.DoubleEquals;
        }
    }
}
