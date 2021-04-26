using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Text;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Binding.NodeKinds;

namespace Uranium.Logging
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
            var expectedText = expectedKind == SyntaxKind.EndOfFile ? "Expected the file to end" : $"Expected: {SyntaxFacts.GetText(expectedKind)}";
            var message = $"JC0003: Unexpected token: `{actualKind.Text}`. `{expectedText}`.";
            Report(span, message);
        }

        public void ReportUnfinishedMultiLineComment(TextSpan span, int position)
        {
            var message = $"JC0004: Unfinished comment at index: {position}.";
            Report(span, message);
        }

        public void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, Type operandType)
        {
            var message = $"JC0005: Unary operator {operatorText} is not defined for {operandType}.";
            Report(span, message);
        }

        public void ReportUndefinedBinaryOperator(TextSpan span, string? operatorText, Type leftType, Type rightType)
        {
            var message = $"JC0006: Binary operator {operatorText} is undefined for {leftType} and {rightType}.";
            Report(span, message);
        }

        public void ReportUndefinedName(TextSpan span, string name)
        {
            var message = $"JC0007: The type or namespace {name} does not exist.";
            Report(span, message);
        }

        public void ReportVariableAlreadyDeclared(TextSpan span, string name)
        {
            var message = $"JC0008: The variable {name} already exists in the current scope!";
            Report(span, message);
        }

        public void ReportCannotConvert(TextSpan span, Type converterType, Type converteetype)
        {
            var message = $"JC0009: Cannot convert from type '{converterType}' to type '{converteetype}'.";
            Report(span, message);
        }

        public void ReportCannotAssign(TextSpan identifier, TextSpan equals, string name)
        {
            var totalSpan = new TextSpan(identifier.Start, identifier.Length + equals.Length + 1);
            var message = $"JC0010: Cannot assign a value to {name} because it is marked as let or const, meaning that it is read only.";
            Report(totalSpan, message);
        }

        public void ReportNoSemiColon(TextSpan span)
        {
            var message = $"JC0011: Line cannot end without a semi colon";
            Report(span, message);
        }
        
        public void ReportCannotStartWithNumber(TextSpan span, string text)
        {
            var message = $"JC0012: A file cannot start with a number. {text}";
            Report(span, message);
        }
    }
}
