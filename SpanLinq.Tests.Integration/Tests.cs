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
    }
}
