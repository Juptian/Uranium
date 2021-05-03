using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Expression;

namespace Uranium.Tests.CodeAnalysis.Parsing
{
    internal sealed class AssertingEnumerator : IDisposable
    {
        private readonly IEnumerator<SyntaxNode> _enumerator;
        private bool _hasError;
        private int _position = 0;

        public AssertingEnumerator(SyntaxNode node)
        {
            _enumerator = Flatten(node).GetEnumerator();
        }

        public void Dispose()
        {
            if(!_hasError)
            {
                while(_enumerator.MoveNext())
                {}
                Assert.False(_enumerator.MoveNext());
            }
            _enumerator.Dispose();
        }

        private bool MarkFailed()
        {
            _hasError = true;
            return false;
        }

        private static IEnumerable<SyntaxNode> Flatten(SyntaxNode node)
        {
            
            var stack = new Stack<SyntaxNode>();
            stack.Push(node);

            while(stack.Count > 0)
            {
                //Removing the node we just got so that we can get it's children specifically
                //Without needing to initialize another variable to keep our index
                var n = stack.Pop();
                yield return n;

                //Forcefully adding nodes, so that we can flatten the ENTIRE tree
                foreach(var child in n.GetChildren().Reverse())
                {
                    stack.Push(child);
                }
            }
        }
        
        public void AssertToken(SyntaxKind kind, string text, bool asSoloOperator = false)
        {
            try
            {
                _position++;
                Assert.True(_enumerator.MoveNext());
                Assert.Equal(kind, _enumerator.Current.Kind);

                var token = Assert.IsType<SyntaxToken>(_enumerator.Current);
                if(asSoloOperator)
                {
                    var asSolo = SyntaxFacts.GetSoloOperator(token);
                    Assert.Equal(text, SyntaxFacts.GetText(asSolo.Kind));
                }
                else
                {
                    Assert.Equal(text, token.Text);
                }
            }
            catch when (MarkFailed())
            {
                throw;
            }
        }

        public void AssertNode(SyntaxKind kind)
        {
            try
            {
                _position++;
                Assert.True(_enumerator.MoveNext());
                Assert.Equal(kind, _enumerator.Current.Kind);
                Assert.IsNotType<SyntaxToken>(_enumerator.Current);
            }
            catch when (MarkFailed())
            {
                throw;
            }
        }

        public void AssertLiteralExpression()
        {
            try
            {
                _position++;
                Assert.True(_enumerator.MoveNext());
                Assert.Equal(SyntaxKind.LiteralExpression, _enumerator.Current.Kind);
                Assert.IsType<LiteralExpressionSyntax>(_enumerator.Current);
            }
            catch when (MarkFailed())
            {
                throw;
            }
        }

        public void AssertNameExpression()
        {
            try
            {
                _position++;
                Assert.True(_enumerator.MoveNext());
                Assert.Equal(SyntaxKind.NameExpression, _enumerator.Current.Kind);
                var token = Assert.IsType<NameExpressionSyntax>(_enumerator.Current);
            }
            catch when (MarkFailed())
            {
                throw;
            }
        }

        public void AssertVariableDeclaration
            (
                SyntaxKind keyword, string keywordName, 
                string initializerName, string initializerValue
            )
        {
            AssertNode(SyntaxKind.VariableDeclaration);
                AssertToken(keyword, keywordName);
                AssertToken(SyntaxKind.IdentifierToken, initializerName);
                AssertToken(SyntaxKind.Equals, "=");
                AssertLiteralExpression();
                    AssertToken(SyntaxKind.NumberToken, initializerValue);
                 
                AssertToken(SyntaxKind.Semicolon, ";");
        }

        public void AssertVariableDeclaration(SyntaxKind keyword, string identifer)
        {
            AssertNode(SyntaxKind.VariableDeclaration);
            AssertToken(keyword, SyntaxFacts.GetText(keyword));
            AssertToken(SyntaxKind.IdentifierToken, identifer);
        }
        public void AssertCondition
            (
            string initializerName,
            SyntaxKind conditionToken, string conditionText
            )
        {
            AssertNode(SyntaxKind.BinaryExpression);
                AssertNameExpression();
                    AssertToken(SyntaxKind.IdentifierToken, initializerName);

                AssertToken(conditionToken, SyntaxFacts.GetText(conditionToken));
                AssertLiteralExpression();
                    AssertToken(SyntaxKind.NumberToken, conditionText);
        }

        public void AssertAssignmentExpression
            (
            string initializerName,
            SyntaxKind soloOperator, string soloText,
            string incrementationText,
            SyntaxKind incrementor
            )
        {
            AssertNode(SyntaxKind.AssignmentExpression);
                AssertToken(SyntaxKind.IdentifierToken, initializerName);
                AssertToken(soloOperator, soloText, true);
                AssertLiteralExpression();
                    AssertToken(SyntaxKind.NumberToken, incrementationText);
                //This one is because in our Assignment expression syntax we save our compound operator
                //So, we have to assert it here as well.
                AssertToken(incrementor, SyntaxFacts.GetText(incrementor));
        }

        public void AssertBlockStatement(bool ignoreStatements)
        {
            AssertNode(SyntaxKind.BlockStatement);
            AssertToken(SyntaxKind.OpenCurlyBrace, "{");
            if(ignoreStatements)
            {
                IgnoreBlockStatements();
            }
            else
            {
                AssertToken(SyntaxKind.CloseCurlyBrace, "}");
            }
        }

        public void IgnoreBlockStatements()
        {
            while(_enumerator.Current.Kind is not SyntaxKind.CloseCurlyBrace)
            {
                Assert.True(_enumerator.MoveNext());
            }
            Assert.True(_enumerator.Current.Kind is SyntaxKind.CloseCurlyBrace);
        }
    }
}
