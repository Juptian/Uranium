using System.Collections.Generic;
using System.Reflection;

namespace Uranium.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public IEnumerable<SyntaxNode> GetChildren()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach(var property in properties)
            {
                if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = property.GetValue(this) as SyntaxNode;
                    yield return child;
                }
                else if (typeof(IEnumerable<SyntaxKind>).IsAssignableFrom(property.PropertyType))
                {
                    var values = property.GetValue(this) as IEnumerable<SyntaxNode>;
                    foreach (var child in values)
                    {
                        yield return child;
                    }
                }
            }
        }
    }
}
