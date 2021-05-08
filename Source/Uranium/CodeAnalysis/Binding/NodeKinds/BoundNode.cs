using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using Uranium.CodeAnalysis.Binding.Statements;

namespace Uranium.CodeAnalysis.Binding.NodeKinds
{
    //Base class
    //Yep that's it!
    internal abstract class BoundNode
    {
        public abstract BoundNodeKind Kind { get; }

        public IEnumerable<(string Name, object Value)> GetProperties()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
            for(int i = 0; i < properties.Length; i++)
            {
                if(properties[i].Name == nameof(Kind) || 
                   properties[i].Name == nameof(BoundBinaryExpression.Op) ||
                   properties[i].Name == nameof(BoundAssignmentExpression.CompoundOperator))
                {
                    continue;
                }

                if (typeof(BoundNode).IsAssignableFrom(properties[i].PropertyType) ||
                    typeof(IEnumerable<BoundNodeKind>).IsAssignableFrom(properties[i].PropertyType))
                {
                    continue;
                }

                var value = properties[i]?.GetValue(this);

                if(value is not null)
                {
                    yield return (properties[i].Name, value);
                }
            }
        }


        //Why forcefully override when I can avoid it 
        //This method works on all children of the class
        //This also allows for a central method, instead of 20 million implementations.
        //This should work fine because metadata
        public IEnumerable<BoundNode> GetChildren()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
            for(int i = 0; i < properties.Length; i++)
            {
                if (typeof(BoundNode).IsAssignableFrom(properties[i].PropertyType))
                {
                    if (properties[i].GetValue(this) is BoundNode child)
                    {
                        yield return child;
                    }
                }
                else if (typeof(IEnumerable<BoundNode>).IsAssignableFrom(properties[i].PropertyType))
                {
                    var children = (properties[i].GetValue(this) as IEnumerable<BoundNode>)?.ToArray() ?? null;
                    if(children is not null)
                    {
                        for(int x = 0; x < children.Length; x++)
                        {
                            if(children[x] is not null)
                            {
                                yield return children[x];
                            }
                        }
                    }
                }
            }
        }

        private static void PrettyPrint(TextWriter writer, BoundNode node, string indent = "", bool isLast = true)
        {
            //Variable to not bother changing the console colour if we're not printing to the console
            //That just makes no sense!
            var isToConsole = writer == Console.Out;
            var marker = isLast ? "└───" : "├───";
            
            writer.Write(indent);
            writer.Write(marker);

            WriteNode(writer, node, isToConsole);
            WriteProperties(writer, node, isToConsole);

            writer.WriteLine();

            if (isToConsole) Console.ResetColor();

            indent += isLast ? "    " : "│   ";

            var lastChild = node.GetChildren().LastOrDefault();
            var children = node.GetChildren().ToArray();
            for(int i = 0; i < children.Length; i++)
            {
                PrettyPrint(writer, children[i], indent, children[i] == lastChild);
            }
        }

        private static void WriteNode(TextWriter writer, BoundNode node, bool isToConsole)
        {
            if(isToConsole)
            {
                Console.ForegroundColor = GetColor(node);
            }
            var text = GetText(node);
            writer.Write(text);
            if(isToConsole)
            {
                Console.ResetColor();
            }
        }

        private static void WriteProperties(TextWriter writer, BoundNode node, bool isToConsole)
        {
            var array = node.GetProperties().ToArray();

            var isFirstProperty = true;

            for(int i = 0; i < array.Length; i++)
            {
                if(isFirstProperty)
                {
                    isFirstProperty = false;
                }
                else
                {
                    if(isToConsole) Console.ForegroundColor = ConsoleColor.DarkGray;
                    writer.Write(",");
                }
                if (isToConsole) Console.ForegroundColor = ConsoleColor.Yellow;

                writer.Write(" " + array[i].Name);

                if (isToConsole) Console.ForegroundColor = ConsoleColor.DarkGray;
                writer.Write(" = ");

                if (isToConsole) Console.ForegroundColor = ConsoleColor.DarkYellow;

                writer.Write(array[i].Value);
            }
        }

        private static ConsoleColor GetColor(BoundNode node)
        {
            if(node is BoundExpression)
            {
                return ConsoleColor.Green;
            }
            if(node is BoundStatement)
            {
                return ConsoleColor.Cyan;
            }
            return ConsoleColor.Gray;
        }

        private static string GetText(BoundNode node)
        {
            if(node is BoundBinaryExpression b)
            {
                return b.Op.Kind.ToString() + "Expression";
            }
            if(node is BoundUnaryExpression u)
            {
                return u.Op.Kind.ToString() + "Expression";
            }

            return node.Kind.ToString();
        }

        public void WriteTo(TextWriter writer)
        {
            writer.WriteLine();
            PrettyPrint(writer, this);
        }

        public override string ToString()
        {
            using var writer = new StringWriter();

            WriteTo(writer);

            return writer.ToString();
        }
    }
}
