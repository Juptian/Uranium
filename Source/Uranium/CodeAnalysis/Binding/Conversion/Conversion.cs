using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Symbols;
using Uranium.CodeAnalysis.Syntax;

namespace Uranium.CodeAnalysis.Binding.Converting
{
    internal sealed class Conversion
    {
        public static readonly Conversion None = new(false, false, false);
        public static readonly Conversion Identity = new(true, true, false);
        public static readonly Conversion Implicit = new(true, false, true);
        public static readonly Conversion Explicit = new(true, false, false);


        private Conversion(bool exists, bool isIdentity, bool isImplicit)
        {
            Exists = exists;
            IsIdentity = isIdentity;
            IsImplicit = isImplicit;
        }

        public bool Exists { get; }
        public bool IsIdentity { get; }
        public bool IsImplicit { get; }
        public bool IsExplicit => Exists && !IsImplicit;

        public static Conversion Classify(TypeSymbol from, TypeSymbol to)
        {
            
            // Invalid casts
            if(from == TypeSymbol.Void || to == TypeSymbol.Void)
            {
                return None;
            }
            if(from == TypeSymbol.Error || to == TypeSymbol.Error)
            {
                return None;
            }

            // Other casts
            if(from == to)
            {
                return Identity;
            }

            if(from == TypeSymbol.Bool || from == TypeSymbol.Int)
            {
                if(to == TypeSymbol.String)
                {
                    return Explicit;
                }
                return Implicit;
            }

            if(from == TypeSymbol.Long)
            {
                if(to == TypeSymbol.Int || to == TypeSymbol.String)
                {
                    return Explicit;
                }
                return None;
            }

            if(from == TypeSymbol.Float)
            {
                if(to == TypeSymbol.Double)
                {
                    return Implicit;
                }
                if(to == TypeSymbol.Long)
                {
                    return None;
                }
                return Explicit;
            }

            if(from == TypeSymbol.Double)
            {
                if(to == TypeSymbol.Long)
                {
                    return None;
                }
                return Explicit;
            }

            if(from == TypeSymbol.Char)
            {
                if(to == TypeSymbol.String || to == TypeSymbol.Int)
                {
                    return Implicit;
                }
                return None;
            }

            if(from == TypeSymbol.String || to == TypeSymbol.String)
            {
                return Explicit;
            }

            return None;
        }

    }
}
