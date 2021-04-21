
using FluentAssertions;
using System;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace SpanLinq.Tests.Unit
{
    public class GeneratorTests : TestBase
    {
        public GeneratorTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void SelectSelectFollowedByToList()
        {
            string userSource = @"
using System;
using System.Linq;
using System.Collections.Generic;

Span<int> span = default;
List<int> list = span.Select(x => x * x).Select(x => x - x).ToList();
list.Select(x => x * x);
";
            var comp = RunGenerator(userSource, out var generatorDiagnostics, out var generated);
            generatorDiagnostics.Verify();
            comp.GetDiagnostics().Verify();
            var file = Assert.Single(generated);
            file.Should().BeIgnoringLineEndings(@"namespace System.Linq
{
    internal static class SpanLinq
    {
        public static SelectSpan<TSource, TResult> Select<TSource, TResult>(this Span<TSource> source, Func<TSource, TResult> selector)
        {
            return new SelectSpan<TSource, TResult>(source, selector);
        }

        public ref struct SelectSpan<TSource, TResult>
        {
            private Span<TSource> source;
            private Func<TSource, TResult> selector;

            public SelectSpan(Span<TSource> source, Func<TSource, TResult> selector)
            {
                this.source = source;
                this.selector = selector;
            }
            
            public int Length => source.Length;

            public Enumerator GetEnumerator()
            {
                return new Enumerator(this);
            }

            public ref struct Enumerator
            {
                private Func<TSource, TResult> selector;
                private Span<TSource>.Enumerator enumerator;

                public Enumerator(SelectSpan<TSource, TResult> outer)
                {
                    selector = outer.selector;
                    enumerator = outer.source.GetEnumerator();
                    Current = default;
                }

                public TResult Current { get; private set; }

                public bool MoveNext()
                {
                    if (!enumerator.MoveNext())
                    {
                        return false;
                    }
                    Current = selector(enumerator.Current);
                    return true;
                }
            }
        }

        public static SelectSelectSpan<SelectSpanTSource, SelectSpanTResult, TResult> Select<SelectSpanTSource, SelectSpanTResult, TResult>(this SelectSpan<SelectSpanTSource, SelectSpanTResult> source, Func<SelectSpanTResult, TResult> selector)
        {
            return new SelectSelectSpan<SelectSpanTSource, SelectSpanTResult, TResult>(source, selector);
        }

        public ref struct SelectSelectSpan<SelectSpanTSource, SelectSpanTResult, TResult>
        {
            private SelectSpan<SelectSpanTSource, SelectSpanTResult> source;
            private Func<SelectSpanTResult, TResult> selector;

            public SelectSelectSpan(SelectSpan<SelectSpanTSource, SelectSpanTResult> source, Func<SelectSpanTResult, TResult> selector)
            {
                this.source = source;
                this.selector = selector;
            }
            
            public int Length => source.Length;

            public Enumerator GetEnumerator()
            {
                return new Enumerator(this);
            }

            public ref struct Enumerator
            {
                private Func<SelectSpanTResult, TResult> selector;
                private SelectSpan<SelectSpanTSource, SelectSpanTResult>.Enumerator enumerator;

                public Enumerator(SelectSelectSpan<SelectSpanTSource, SelectSpanTResult, TResult> outer)
                {
                    selector = outer.selector;
                    enumerator = outer.source.GetEnumerator();
                    Current = default;
                }

                public TResult Current { get; private set; }

                public bool MoveNext()
                {
                    if (!enumerator.MoveNext())
                    {
                        return false;
                    }
                    Current = selector(enumerator.Current);
                    return true;
                }
            }
        }

        public static System.Collections.Generic.List<TResult> ToList<SelectSpanTSource, SelectSpanTResult, TResult>(this SelectSelectSpan<SelectSpanTSource, SelectSpanTResult, TResult> source)
        {
            var list = new System.Collections.Generic.List<TResult>(source.Length);
            foreach (var item in source)
            {
                list.Add(item);
            }
            return list;
        }
    }
}");
        }

        [Fact]
        public void SharesSameGeneratedTypesIfPossible()
        {
            string userSource = @"
using System;
using System.Linq;

Span<int> span = default;
span.Select(x => x * x).Select(x => x - x).ToList();
span.Select(x => (long)x).Select(x => (short)x).ToList();
";
            var comp = RunGenerator(userSource, out var generatorDiagnostics, out var generated);
            generatorDiagnostics.Verify();
            comp.GetDiagnostics().Verify();
            var file = Assert.Single(generated);
            file.Should().BeIgnoringLineEndings(@"namespace System.Linq
{
    internal static class SpanLinq
    {
        public static SelectSpan<TSource, TResult> Select<TSource, TResult>(this Span<TSource> source, Func<TSource, TResult> selector)
        {
            return new SelectSpan<TSource, TResult>(source, selector);
        }

        public ref struct SelectSpan<TSource, TResult>
        {
            private Span<TSource> source;
            private Func<TSource, TResult> selector;

            public SelectSpan(Span<TSource> source, Func<TSource, TResult> selector)
            {
                this.source = source;
                this.selector = selector;
            }
            
            public int Length => source.Length;

            public Enumerator GetEnumerator()
            {
                return new Enumerator(this);
            }

            public ref struct Enumerator
            {
                private Func<TSource, TResult> selector;
                private Span<TSource>.Enumerator enumerator;

                public Enumerator(SelectSpan<TSource, TResult> outer)
                {
                    selector = outer.selector;
                    enumerator = outer.source.GetEnumerator();
                    Current = default;
                }

                public TResult Current { get; private set; }

                public bool MoveNext()
                {
                    if (!enumerator.MoveNext())
                    {
                        return false;
                    }
                    Current = selector(enumerator.Current);
                    return true;
                }
            }
        }

        public static SelectSelectSpan<SelectSpanTSource, SelectSpanTResult, TResult> Select<SelectSpanTSource, SelectSpanTResult, TResult>(this SelectSpan<SelectSpanTSource, SelectSpanTResult> source, Func<SelectSpanTResult, TResult> selector)
        {
            return new SelectSelectSpan<SelectSpanTSource, SelectSpanTResult, TResult>(source, selector);
        }

        public ref struct SelectSelectSpan<SelectSpanTSource, SelectSpanTResult, TResult>
        {
            private SelectSpan<SelectSpanTSource, SelectSpanTResult> source;
            private Func<SelectSpanTResult, TResult> selector;

            public SelectSelectSpan(SelectSpan<SelectSpanTSource, SelectSpanTResult> source, Func<SelectSpanTResult, TResult> selector)
            {
                this.source = source;
                this.selector = selector;
            }
            
            public int Length => source.Length;

            public Enumerator GetEnumerator()
            {
                return new Enumerator(this);
            }

            public ref struct Enumerator
            {
                private Func<SelectSpanTResult, TResult> selector;
                private SelectSpan<SelectSpanTSource, SelectSpanTResult>.Enumerator enumerator;

                public Enumerator(SelectSelectSpan<SelectSpanTSource, SelectSpanTResult, TResult> outer)
                {
                    selector = outer.selector;
                    enumerator = outer.source.GetEnumerator();
                    Current = default;
                }

                public TResult Current { get; private set; }

                public bool MoveNext()
                {
                    if (!enumerator.MoveNext())
                    {
                        return false;
                    }
                    Current = selector(enumerator.Current);
                    return true;
                }
            }
        }

        public static System.Collections.Generic.List<TResult> ToList<SelectSpanTSource, SelectSpanTResult, TResult>(this SelectSelectSpan<SelectSpanTSource, SelectSpanTResult, TResult> source)
        {
            var list = new System.Collections.Generic.List<TResult>(source.Length);
            foreach (var item in source)
            {
                list.Add(item);
            }
            return list;
        }
    }
}");
        }

        [Fact]
        public void WhereSelectWhereFollowedByToList()
        {
            string userSource = @"
using System;
using System.Linq;
using System.Collections.Generic;

Span<int> span = default;
List<string> list = span.Where(x => true).Select(x => x.ToString()).Where(x => false).ToList();
";
            var comp = RunGenerator(userSource, out var generatorDiagnostics, out var generated);
            generatorDiagnostics.Verify();
            comp.GetDiagnostics().Verify();
            var file = Assert.Single(generated);
            file.Should().BeIgnoringLineEndings(@"namespace System.Linq
{
    internal static class SpanLinq
    {
        public static WhereSpan<T> Where<T>(this Span<T> source, Func<T, bool> predicate)
        {
            return new WhereSpan<T>(source, predicate);
        }

        public ref struct WhereSpan<T>
        {
            private Span<T> source;
            private Func<T, bool> predicate;

            public WhereSpan(Span<T> source, Func<T, bool> predicate)
            {
                this.source = source;
                this.predicate = predicate;
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(this);
            }

            public ref struct Enumerator
            {
                private Func<T, bool> predicate;
                private Span<T>.Enumerator enumerator;

                public Enumerator(WhereSpan<T> outer)
                {
                    predicate = outer.predicate;
                    enumerator = outer.source.GetEnumerator();
                }

                public T Current => enumerator.Current;

                public bool MoveNext()
                {
                    while (enumerator.MoveNext())
                    {
                        if (predicate(enumerator.Current))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
        }

        public static SelectWhereSpan<TSource, TResult> Select<TSource, TResult>(this WhereSpan<TSource> source, Func<TSource, TResult> selector)
        {
            return new SelectWhereSpan<TSource, TResult>(source, selector);
        }

        public ref struct SelectWhereSpan<TSource, TResult>
        {
            private WhereSpan<TSource> source;
            private Func<TSource, TResult> selector;

            public SelectWhereSpan(WhereSpan<TSource> source, Func<TSource, TResult> selector)
            {
                this.source = source;
                this.selector = selector;
            }
            
            public Enumerator GetEnumerator()
            {
                return new Enumerator(this);
            }

            public ref struct Enumerator
            {
                private Func<TSource, TResult> selector;
                private WhereSpan<TSource>.Enumerator enumerator;

                public Enumerator(SelectWhereSpan<TSource, TResult> outer)
                {
                    selector = outer.selector;
                    enumerator = outer.source.GetEnumerator();
                    Current = default;
                }

                public TResult Current { get; private set; }

                public bool MoveNext()
                {
                    if (!enumerator.MoveNext())
                    {
                        return false;
                    }
                    Current = selector(enumerator.Current);
                    return true;
                }
            }
        }

        public static WhereSelectWhereSpan<TSource, TResult> Where<TSource, TResult>(this SelectWhereSpan<TSource, TResult> source, Func<TResult, bool> predicate)
        {
            return new WhereSelectWhereSpan<TSource, TResult>(source, predicate);
        }

        public ref struct WhereSelectWhereSpan<TSource, TResult>
        {
            private SelectWhereSpan<TSource, TResult> source;
            private Func<TResult, bool> predicate;

            public WhereSelectWhereSpan(SelectWhereSpan<TSource, TResult> source, Func<TResult, bool> predicate)
            {
                this.source = source;
                this.predicate = predicate;
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(this);
            }

            public ref struct Enumerator
            {
                private Func<TResult, bool> predicate;
                private SelectWhereSpan<TSource, TResult>.Enumerator enumerator;

                public Enumerator(WhereSelectWhereSpan<TSource, TResult> outer)
                {
                    predicate = outer.predicate;
                    enumerator = outer.source.GetEnumerator();
                }

                public TResult Current => enumerator.Current;

                public bool MoveNext()
                {
                    while (enumerator.MoveNext())
                    {
                        if (predicate(enumerator.Current))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
        }

        public static System.Collections.Generic.List<TResult> ToList<TSource, TResult>(this WhereSelectWhereSpan<TSource, TResult> source)
        {
            var list = new System.Collections.Generic.List<TResult>();
            foreach (var item in source)
            {
                list.Add(item);
            }
            return list;
        }
    }
}");
        }

        [Fact]
        public void SelectAndWhereFollowedByToArray()
        {
            string userSource = @"
using System;
using System.Linq;

Span<int> span = default;
span.Select(x => x.ToString()).ToArray();
span.Where(x => x > 0).ToArray();
";
            var comp = RunGenerator(userSource, out var generatorDiagnostics, out var generated);
            generatorDiagnostics.Verify();
            comp.GetDiagnostics().Verify();
            var file = Assert.Single(generated);
            file.Should().BeIgnoringLineEndings(@"namespace System.Linq
{
    internal static class SpanLinq
    {
        public static SelectSpan<TSource, TResult> Select<TSource, TResult>(this Span<TSource> source, Func<TSource, TResult> selector)
        {
            return new SelectSpan<TSource, TResult>(source, selector);
        }

        public ref struct SelectSpan<TSource, TResult>
        {
            private Span<TSource> source;
            private Func<TSource, TResult> selector;

            public SelectSpan(Span<TSource> source, Func<TSource, TResult> selector)
            {
                this.source = source;
                this.selector = selector;
            }
            
            public int Length => source.Length;

            public Enumerator GetEnumerator()
            {
                return new Enumerator(this);
            }

            public ref struct Enumerator
            {
                private Func<TSource, TResult> selector;
                private Span<TSource>.Enumerator enumerator;

                public Enumerator(SelectSpan<TSource, TResult> outer)
                {
                    selector = outer.selector;
                    enumerator = outer.source.GetEnumerator();
                    Current = default;
                }

                public TResult Current { get; private set; }

                public bool MoveNext()
                {
                    if (!enumerator.MoveNext())
                    {
                        return false;
                    }
                    Current = selector(enumerator.Current);
                    return true;
                }
            }
        }

        public static WhereSpan<T> Where<T>(this Span<T> source, Func<T, bool> predicate)
        {
            return new WhereSpan<T>(source, predicate);
        }

        public ref struct WhereSpan<T>
        {
            private Span<T> source;
            private Func<T, bool> predicate;

            public WhereSpan(Span<T> source, Func<T, bool> predicate)
            {
                this.source = source;
                this.predicate = predicate;
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(this);
            }

            public ref struct Enumerator
            {
                private Func<T, bool> predicate;
                private Span<T>.Enumerator enumerator;

                public Enumerator(WhereSpan<T> outer)
                {
                    predicate = outer.predicate;
                    enumerator = outer.source.GetEnumerator();
                }

                public T Current => enumerator.Current;

                public bool MoveNext()
                {
                    while (enumerator.MoveNext())
                    {
                        if (predicate(enumerator.Current))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
        }

        public static TResult[] ToArray<TSource, TResult>(this SelectSpan<TSource, TResult> source)
        {
            var array = new TResult[source.Length];
            int i = 0;
            foreach (var item in source)
            {
                array[i] = item;
                i++;
            }
            return array;
        }

        public static T[] ToArray<T>(this WhereSpan<T> source)
        {
            var list = new System.Collections.Generic.List<T>();
            foreach (var item in source)
            {
                list.Add(item);
            }
            return list.ToArray();
        }
    }
}");
        }
    }
}
