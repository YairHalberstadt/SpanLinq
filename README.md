# SpanLinq
Linq for Span<T> using SourceGenerators

## What it does

Provides a set of linq extension methods on `Span<T>` and `ReadOnlySpan<T>`. The extension methods maintain the same semantics as the framework provided linq for `IEnumerable<T>`. They are lazy and can be chained: `span.Select(x => x * x).Where(x > 100).ToList()` will work, and will only allocate one `List` - no intermediary collections.

Not all of linq is provided, but the most common methods are. If the method you would like isn't here, just open an issue, or even a PR. Till then, you can always realize your span via `ToList` and then use ordinary linq.

## How it works

Because `Span<T>` and `ReadOnlySpan<T>` are ref structs it's impossible to abstract over them. This means that you can't write a single extension method that workd for both `Span<T>` and `ReadOnlySpan<T>`, and even if you did write both, the return type wouldn't have an extension method, preventing chaining. 

Instead SpanLinq uses Source Generators to generate extension methods on the fly, as they are used. This has some advantages, but also disadvantages:

**advantages**

- generated methods are optimized for the exact input type, and so almost cost free.
- only generate methods you actually use, limiting code bloat.

**disadvantages**

- potentially slow compile times, and IDE lag. This is generally fine, but gets worse the longer your longest Linq chain is. Future development could improve this.
- poor code completion. I hope to improve this in the future with a custom completion provider.
- potential code bloat, since methods often can't be reused, you could theoretically end up generating hundreds of almost identical methods. This is unlikely to happen except on the largest projects.

## Available methods

- `.Select<TSource, TResult>(Func<TSource, TResult> selector)`
- `.Where<T>(Func<T, bool> predicate)`
- `.ToList<T>()`
- `.ToArray<T>()`
- `.Skip<T>(int count)`
- `.Take<T>(int count)`
- `.Reverse<T>()`
- `.Count<T>()`
- `.Any<T>()`
- `.Any<T>(Func<T, bool> predicate)`
- `.All<T>()`
- `.All<T>(Func<T, bool> predicate)`
- `.FirstOrDefault<T>()`
- `.FirstOrDefault<T>(Func<T, bool> predicate)`
- `.First<T>()`
- `.First<T>(Func<T, bool> predicate)`
- `.SingleOrDefault<T>()`
- `.SingleOrDefault<T>(Func<T, bool> predicate)`
- `.Single<T>()`
- `.Single<T>(Func<T, bool> predicate)`
- `.LastOrDefault<T>()`
- `.LastOrDefault<T>(Func<T, bool> predicate)`
- `.Last<T>()`
- `.Last<T>(Func<T, bool> predicate)`