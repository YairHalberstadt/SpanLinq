
using FluentAssertions;
using FluentAssertions.Primitives;

namespace SpanLinq.Tests.Unit
{
    public static class AssertionExtensions
    {
        public static AndConstraint<StringAssertions> BeIgnoringLineEndings(this StringAssertions stringAssertions, string expected, string because = "", params object[] becauseArgs)
        {
            return stringAssertions.Subject.Replace("\r\n", "\n").Should().Be(expected.Replace("\r\n", "\n"));
        }
    }
}
