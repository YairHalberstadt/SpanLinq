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
        public void TestTakeLessThan0()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new int[] {  }, span.Take(-1).ToList());
        }

        [Fact]
        public void TestWhereTakeLessThan0()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new int[] {  }, span.Where(x => x % 2 == 1).Take(-1).ToList());
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
        public void TestSkipLessThan0()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new int[] { 1, 2, 3, 4, 5 }, span.Skip(-1).ToList());
        }

        [Fact]
        public void TestWhereSkipLessThan0()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new int[] { 1, 3, 5 }, span.Where(x => x % 2 == 1).Skip(-1).ToList());
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

        [Fact]
        public void TestFirstOrDefaultEmpty()
        {
            Span<int> span = stackalloc int[] { };
            Assert.Equal(default, span.FirstOrDefault());
        }

        [Fact]
        public void TestFirstOrDefaultHasValues()
        {
            Span<int> span = stackalloc int[] { 1 , 2 };
            Assert.Equal(1, span.FirstOrDefault());
        }

        [Fact]
        public void TestSelectFirstOrDefaultEmpty()
        {
            Span<int> span = stackalloc int[] { };
            Assert.Equal(default, span.Select(x => x * x).FirstOrDefault());
        }

        [Fact]
        public void TestSelectFirstOrDefaultHasValues()
        {
            Span<int> span = stackalloc int[] { 2 };
            Assert.Equal(4, span.Select(x => x * x).FirstOrDefault());
        }

        [Fact]
        public void TestWhereFirstOrDefaultEmpty()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5, 6 };
            Assert.Equal(default, span.Where(x => x > 10).FirstOrDefault());
        }

        [Fact]
        public void TestWhereFirstOrDefaultHasValues()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5, 6 };
            Assert.Equal(5, span.Where(x => x > 4).FirstOrDefault());
        }

        [Fact]
        public void TestFirstOrDefaultPredicateEmpty()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(default, span.FirstOrDefault(x => x > 10));
        }

        [Fact]
        public void TestFirstOrDefaultPredicateHasValues()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(5, span.FirstOrDefault(x => x > 4));
        }

        [Fact]
        public void TestSelectFirstOrDefaultPredicateEmpty()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(default, span.Select(x => x + 10).FirstOrDefault(x => x < 10));
        }

        [Fact]
        public void TestSelectFirstOrDefaultPredicateHasValues()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(11, span.Select(x => x + 10).FirstOrDefault(x => x > 10));
        }

        [Fact]
        public void TestWhereFirstOrDefaultPredicateEmpty()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(default, span.Where(x => x > 3).FirstOrDefault(x => x <= 3));
        }

        [Fact]
        public void TestWhereFirstOrDefaultPredicateHasValues()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(5, span.Where(x => x > 3).FirstOrDefault(x => x > 4));
        }

        [Fact]
        public void TestFirstEmpty()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { };
                return span.First();
            });
        }

        [Fact]
        public void TestFirstHasValues()
        {
            Span<int> span = stackalloc int[] { 1, 2 };
            Assert.Equal(1, span.First());
        }

        [Fact]
        public void TestSelectFirstEmpty()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { };
                return span.Select(x => x * x).First();
            });
        }

        [Fact]
        public void TestSelectFirstHasValues()
        {
            Span<int> span = stackalloc int[] { 2 };
            Assert.Equal(4, span.Select(x => x * x).First());
        }

        [Fact]
        public void TestWhereFirstEmpty()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5, 6 };
                return span.Where(x => x > 10).First();
            });
        }

        [Fact]
        public void TestWhereFirstHasValues()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5, 6 };
            Assert.Equal(5, span.Where(x => x > 4).First());
        }

        [Fact]
        public void TestFirstPredicateEmpty()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
                return span.First(x => x > 10);
            });
        }

        [Fact]
        public void TestFirstPredicateHasValues()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(5, span.First(x => x > 4));
        }

        [Fact]
        public void TestSelectFirstPredicateEmpty()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
                return span.Select(x => x + 10).First(x => x < 10);
            });
        }

        [Fact]
        public void TestSelectFirstPredicateHasValues()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(11, span.Select(x => x + 10).First(x => x > 10));
        }

        [Fact]
        public void TestWhereFirstPredicateEmpty()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
                return span.Where(x => x > 3).First(x => x <= 3);
            });
        }

        [Fact]
        public void TestWhereFirstPredicateHasValues()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(5, span.Where(x => x > 3).First(x => x > 4));
        }

        [Fact]
        public void TestSingleEmpty()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { };
                return span.Single();
            });
        }

        [Fact]
        public void TestSingleOneElement()
        {
            Span<int> span = stackalloc int[] { 1 };
            Assert.Equal(1, span.Single());
        }

        [Fact]
        public void TestSingleMultipleElements()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { 1, 2 };
                return span.Single();
            });
        }

        [Fact]
        public void TestSelectSingleEmpty()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { };
                return span.Select(x => x.ToString()).Single();
            });
        }

        [Fact]
        public void TestSelectSingleOneElement()
        {
            Span<int> span = stackalloc int[] { 1 };
            Assert.Equal("1", span.Select(x => x.ToString()).Single());
        }

        [Fact]
        public void TestSelectSingleMultipleElements()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { 1, 2 };
                return span.Select(x => x.ToString()).Single();
            });
        }

        [Fact]
        public void TestWhereSingleEmpty()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
                return span.Where(x => x > 5).Single();
            });
        }

        [Fact]
        public void TestWhereSingleOneElement()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(5, span.Where(x => x > 4).Single());
        }

        [Fact]
        public void TestWhereSingleMultipleElements()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
                return span.Where(x => x > 3).Single();
            });
        }

        [Fact]
        public void TestSingleOrDefaultEmpty()
        {
            Span<int> span = stackalloc int[] { };
            Assert.Equal(default, span.SingleOrDefault());
        }

        [Fact]
        public void TestSingleOrDefaultOneElement()
        {
            Span<int> span = stackalloc int[] { 1 };
            Assert.Equal(1, span.SingleOrDefault());
        }

        [Fact]
        public void TestSingleOrDefaultMultipleElements()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { 1, 2 };
                return span.SingleOrDefault();
            });
        }

        [Fact]
        public void TestSelectSingleOrDefaultEmpty()
        {
            Span<int> span = stackalloc int[] { };
            Assert.Equal(default, span.Select(x => x.ToString()).SingleOrDefault());
        }

        [Fact]
        public void TestSelectSingleOrDefaultOneElement()
        {
            Span<int> span = stackalloc int[] { 1 };
            Assert.Equal("1", span.Select(x => x.ToString()).SingleOrDefault());
        }

        [Fact]
        public void TestSelectSingleOrDefaultMultipleElements()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { 1, 2 };
                return span.Select(x => x.ToString()).SingleOrDefault();
            });
        }

        [Fact]
        public void TestWhereSingleOrDefaultEmpty()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(default, span.Where(x => x > 5).SingleOrDefault());
        }

        [Fact]
        public void TestWhereSingleOrDefaultOneElement()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(5, span.Where(x => x > 4).SingleOrDefault());
        }

        [Fact]
        public void TestWhereSingleOrDefaultMultipleElements()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
                return span.Where(x => x > 3).SingleOrDefault();
            });
        }

        [Fact]
        public void TestAllFalse()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.False(span.All(x => x < 4));
        }

        [Fact]
        public void TestAllTrue()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.True(span.All(x => x < 6));
        }

        [Fact]
        public void TestSelectAllFalse()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.False(span.Select(x => x + 10).All(x => x < 14));
        }

        [Fact]
        public void TestSelectAllTrue()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.True(span.Select(x => x + 10).All(x => x > 10));
        }

        [Fact]
        public void TestWhereAllFalse()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.False(span.Where(x => x > 3).All(x => x > 4));
        }

        [Fact]
        public void TestWhereAllTrue()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.True(span.Where(x => x > 3).All(x => x > 3));
        }

        [Fact]
        public void TestAllOnEmptySpanReturnsTrue()
        {
            Span<int> span = stackalloc int[] { };
            Assert.True(span.All(x => x > 10));
        }

        [Fact]
        public void TestLastOrDefaultEmpty()
        {
            Span<int> span = stackalloc int[] { };
            Assert.Equal(default, span.LastOrDefault());
        }

        [Fact]
        public void TestLastOrDefaultHasValues()
        {
            Span<int> span = stackalloc int[] { 1, 2 };
            Assert.Equal(2, span.LastOrDefault());
        }

        [Fact]
        public void TestSelectLastOrDefaultEmpty()
        {
            Span<int> span = stackalloc int[] { };
            Assert.Equal(default, span.Select(x => x * x).LastOrDefault());
        }

        [Fact]
        public void TestSelectLastOrDefaultHasValues()
        {
            Span<int> span = stackalloc int[] { 2, 3 };
            Assert.Equal(9, span.Select(x => x * x).LastOrDefault());
        }

        [Fact]
        public void TestWhereLastOrDefaultEmpty()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5, 6 };
            Assert.Equal(default, span.Where(x => x > 10).LastOrDefault());
        }

        [Fact]
        public void TestWhereLastOrDefaultHasValues()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5, 6 };
            Assert.Equal(3, span.Where(x => x < 4).LastOrDefault());
        }

        [Fact]
        public void TestLastOrDefaultPredicateEmpty()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(default, span.LastOrDefault(x => x > 10));
        }

        [Fact]
        public void TestLastOrDefaultPredicateHasValues()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(3, span.LastOrDefault(x => x < 4));
        }

        [Fact]
        public void TestSelectLastOrDefaultPredicateEmpty()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(default, span.Select(x => x + 10).LastOrDefault(x => x < 10));
        }

        [Fact]
        public void TestSelectLastOrDefaultPredicateHasValues()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(13, span.Select(x => x + 10).LastOrDefault(x => x < 14));
        }

        [Fact]
        public void TestWhereLastOrDefaultPredicateEmpty()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(default, span.Where(x => x > 3).LastOrDefault(x => x <= 3));
        }

        [Fact]
        public void TestWhereLastOrDefaultPredicateHasValues()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(3, span.Where(x => x % 2 == 1).LastOrDefault(x => x < 5));
        }

        [Fact]
        public void TestLastEmpty()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { };
                return span.Last();
            });
        }

        [Fact]
        public void TestLastHasValues()
        {
            Span<int> span = stackalloc int[] { 1, 2 };
            Assert.Equal(2, span.Last());
        }

        [Fact]
        public void TestSelectLastEmpty()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { };
                return span.Select(x => x * x).Last();
            });
        }

        [Fact]
        public void TestSelectLastHasValues()
        {
            Span<int> span = stackalloc int[] { 2, 3 };
            Assert.Equal(9, span.Select(x => x * x).Last());
        }

        [Fact]
        public void TestWhereLastEmpty()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5, 6 };
                return span.Where(x => x > 10).Last();
            });
        }

        [Fact]
        public void TestWhereLastHasValues()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5, 6 };
            Assert.Equal(3, span.Where(x => x < 4).Last());
        }

        [Fact]
        public void TestWhereLastHasValues2()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5, 6 };
            Assert.Equal(6, span.Where(x => x > 4).Last());
        }

        [Fact]
        public void TestLastPredicateEmpty()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
                return span.Last(x => x > 10);
            });
        }

        [Fact]
        public void TestLastPredicateHasValues()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(3, span.Last(x => x < 4));
        }

        [Fact]
        public void TestSelectLastPredicateEmpty()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
                return span.Select(x => x + 10).Last(x => x < 10);
            });
        }

        [Fact]
        public void TestSelectLastPredicateHasValues()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(12, span.Select(x => x + 10).Last(x => x < 13));
        }

        [Fact]
        public void TestWhereLastPredicateEmpty()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
                return span.Where(x => x > 3).Last(x => x <= 3);
            });
        }

        [Fact]
        public void TestWhereLastPredicateHasValues()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(3, span.Where(x => x % 2 == 1).Last(x => x < 5));
        }

        [Fact]
        public void TestReverseToList()
        {
            ReadOnlySpan<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 5, 4, 3, 2, 1}, span.Reverse().ToList());
        }

        [Fact]
        public void TestReverseLast()
        {
            ReadOnlySpan<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(1, span.Reverse().Last());
        }

        [Fact]
        public void TestSelectReverseToList()
        {
            ReadOnlySpan<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { "5", "4", "3", "2", "1" }, span.Select(x => x.ToString()).Reverse().ToList());
        }

        [Fact]
        public void TestWhereReverseToList()
        {
            ReadOnlySpan<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 5, 3, 1 }, span.Where(x => x % 2 == 1).Reverse().ToList());
        }

        [Fact]
        public void TestReverseSelectToList()
        {
            ReadOnlySpan<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 25, 16, 9, 4, 1 }, span.Reverse().Select(x => x * x).ToList());
        }

        [Fact]
        public void TestReverseTakeToList()
        {
            ReadOnlySpan<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 5, 4, 3 }, span.Reverse().Take(3).ToList());
        }

        [Fact]
        public void TestReverseSkipToList()
        {
            ReadOnlySpan<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 2, 1 }, span.Reverse().Skip(3).ToList());
        }

        [Fact]
        public void TestWhereReverseTakeToList()
        {
            ReadOnlySpan<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 5, 3 }, span.Where(x => x % 2 == 1).Reverse().Take(2).ToList());
        }

        [Fact]
        public void TestWhereReverseSkipToList()
        {
            ReadOnlySpan<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(new[] { 1 }, span.Where(x => x % 2 == 1).Reverse().Skip(2).ToList());
        }
    }
}
