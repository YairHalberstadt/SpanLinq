using System;
using Xunit;
using System.Linq;

namespace SpanLinq.Tests.Integration
{
    public class Tests
    {
        [Fact]
        public void TestSelectToList()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 1, 4, 9, 16, 25 }, span.Select(x => x * x).ToList());
        }

        [Fact]
        public void TestSelectToArray()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 1, 4, 9, 16, 25 }, span.Select(x => x * x).ToArray());
        }

        [Fact]
        public void TestSelectWhereToList()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 9, 16, 25 }, span.Select(x => x * x).Where(x => x > 5).ToList());
        }

        [Fact]
        public void TestSelectWhereToArray()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 9, 16, 25 }, span.Select(x => x * x).Where(x => x > 5).ToArray());
        }

        [Fact]
        public void TestSelectWhereSelectToList()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { "9", "16", "25" }, span.Select(x => x * x).Where(x => x > 5).Select(x => x.ToString()).ToList());
        }

        [Fact]
        public void TestWhereSelectWhereToList()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 9, 16 }, span.Where(x => x < 5).Select(x => x * x).Where(x => x > 5).ToList());
        }

        [Fact]
        public void TestReadOnlySpan()
        {
            ReadOnlySpan<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 1, 4, 9, 16, 25 }, span.Select(x => x * x).ToList());
        }

        [Fact]
        public void TestTake()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 1, 2, 3 }, span.Take(3).ToList());
        }

        [Fact]
        public void TestSelectTake()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 1, 4, 9 }, span.Select(x => x * x).Take(3).ToList());
        }

        [Fact]
        public void TestWhereTake()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 1, 3 }, span.Where(x => x % 2 == 1).Take(2).ToList());
        }

        [Fact]
        public void TestTakeGreaterThanLength()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 1, 2, 3, 4, 5 }, span.Take(6).ToList());
        }

        [Fact]
        public void TestWhereTakeGreaterThanLength()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 1, 3, 5 }, span.Where(x => x % 2 == 1).Take(4).ToList());
        }

        [Fact]
        public void TestSkip()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 3, 4, 5 }, span.Skip(2).ToList());
        }

        [Fact]
        public void TestSelectSkip()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 9, 16, 25 }, span.Select(x => x * x).Skip(2).ToList());
        }

        [Fact]
        public void TestWhereSkip()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 3, 5 }, span.Where(x => x % 2 == 1).Skip(1).ToList());
        }

        [Fact]
        public void TestSkipGreaterThanLength()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new int [] { }, span.Skip(6).ToList());
        }

        [Fact]
        public void TestWhereSkipGreaterThanLength()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new int[] { }, span.Where(x => x % 2 == 1).Skip(4).ToList());
        }

        [Fact]
        public void TestCount()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(5, span.Count());
        }

        [Fact]
        public void TestSelectCount()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(5, span.Select(x => x * x).Count());
        }

        [Fact]
        public void TestWhereCount()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(3, span.Where(x => x % 2 == 1).Count());
        }

        [Fact]
        public void TestAnyFalse()
        {
            Span<int> span = stackalloc int[] {  };
            Assert.False(span.Any());
        }

        [Fact]
        public void TestAnyTrue()
        {
            Span<int> span = stackalloc int[] { 1 };
            Assert.True(span.Any());
        }

        [Fact]
        public void TestSelectAnyFalse()
        {
            Span<int> span = stackalloc int[] { };
            Assert.False(span.Select(x => x * x).Any());
        }

        [Fact]
        public void TestSelectAnyTrue()
        {
            Span<int> span = stackalloc int[] { 1 };
            Assert.True(span.Select(x => x * x).Any());
        }

        [Fact]
        public void TestWhereAnyFalse()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.False(span.Where(x => x > 10).Any());
        }

        [Fact]
        public void TestWhereAnyTrue()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.True(span.Where(x => x > 4).Any());
        }

        [Fact]
        public void TestAnyPredicateFalse()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5};
            Assert.False(span.Any(x => x > 10));
        }

        [Fact]
        public void TestAnyPredicateTrue()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.True(span.Any(x => x > 4));
        }

        [Fact]
        public void TestSelectAnyPredicateFalse()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.False(span.Select(x => x + 10).Any(x => x < 10));
        }

        [Fact]
        public void TestSelectAnyPredicateTrue()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.True(span.Select(x => x + 10).Any(x => x > 10));
        }

        [Fact]
        public void TestWhereAnyPredicateFalse()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.False(span.Where(x => x > 3).Any(x => x <= 3));
        }

        [Fact]
        public void TestWhereAnyPredicateTrue()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.True(span.Where(x => x > 3).Any(x => x > 4));
        }
    }
}
