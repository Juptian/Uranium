using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Uranium.CodeAnalysis.Syntax;


namespace Uranium.CodeAnalysis.Parsing.ParserSupport.Expression
{
    public abstract class SeparatedSyntaxList
    {
        public abstract ImmutableArray<SyntaxNode> GetWithSeparators();
    }

    public sealed class SeparatedSyntaxList<T> : SeparatedSyntaxList, IEnumerable<T>
        where T : SyntaxNode
    {
        private readonly ImmutableArray<SyntaxNode> _nodesAndSeparators;

        public SeparatedSyntaxList(ImmutableArray<SyntaxNode> separatorsAndNodes) 
        {
            _nodesAndSeparators = separatorsAndNodes;
        }

        // sum(1, 2, 3, 4, 5)
        // would have 9 Syntax Tokens, being:
        // '1' | ',' | '3' | ',' | '4' | ',' | '5' |
        // So if we add one, so that there is an even amount, making up for the "missing" comma
        // Then divide by two, we have the true amount of params
        public int Count => (_nodesAndSeparators.Length + 1) / 2;

        public T this[int index] => (T) _nodesAndSeparators[index * 2];

        public SyntaxToken? GetSeparator(int index)
        {
            if (index == Count - 1)
            {
                return null;
            }
            return (SyntaxToken)_nodesAndSeparators[index * 2 - 1];
        }

        public override ImmutableArray<SyntaxNode> GetWithSeparators() => _nodesAndSeparators;

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; i++) 
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
