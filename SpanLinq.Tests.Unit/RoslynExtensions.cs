using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using Xunit;

namespace SpanLinq.Tests.Unit
{
    public static class RoslynExtensions
    {
        public static void Verify(
            this IEnumerable<Diagnostic> diagnostics,
            params DiagnosticResult[] expected) => DiagnosticVerifier.VerifyDiagnostics(diagnostics, expected);
    }
}
