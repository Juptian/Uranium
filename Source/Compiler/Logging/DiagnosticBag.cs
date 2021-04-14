using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.CodeAnalysis.Text;
using Compiler.CodeAnalysis.Syntax;
using Compiler.CodeAnalysis.Binding.NodeKinds;

namespace Compiler.Logging
{
    internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics = new();

        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.AsReadOnly().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void Report(TextSpan span,string message)
        {
            var diag = new Diagnostic(span, message);
            _diagnostics.Add(diag);
        }

        public void Concat(DiagnosticBag other) => _diagnostics.AddRange(other._diagnostics);

        public void AddRange(DiagnosticBag other) => Concat(other);

        public void ReportNumberStartWithUnderscore(TextSpan span, string text, Type type)
        {
            var message = $"JC0001: {text} : {type} cannot start with an underscore!";
            Report(span, message);
        }

        public void ReportInvalidNumber(TextSpan span, string text, Type type)
        {
            var message = $"JC0002: at {type} {text} : Could not parse {text} to {type}.";
            Report(span, message);
        }

        public void ReportInvalidToken(TextSpan span, SyntaxToken actualKind, SyntaxKind expectedKind)
        {
            var message = $"JC0003: Unexpected token: {actualKind}. Expected: {expectedKind}.";
            Report(span, message);
        }

        public void ReportUnfinishedMultiLineComment(TextSpan span, int position)
        {
            var message = $"JC0004: Unfinished comment at index: {position}.";
            Report(span, message);
        }

        public void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, Type operandType)
        {
            var message = $"JC0005: Unary operator {operatorText} is not defined for {operandType}";
            Report(span, message);
        }

        internal void ReportUndefinedBinaryOperator(TextSpan span, string? operatorText, Type leftType, Type rightType)
        {
            var message = $"JC0006: Binary operator {operatorText} is undefined for {leftType} and {rightType}";
            Report(span, message);
        }

        internal void ReportUndefinedName(TextSpan span, string name)
        {
            var message = $"JC0007: The type or namespace {name} does not exist";
            Report(span, message);
        }
    }
}
