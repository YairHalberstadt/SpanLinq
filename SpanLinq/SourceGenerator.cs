﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using static SpanLinq.Method;

#nullable enable
namespace SpanLinq
{
    [Generator]
    public sealed class SourceGenerator : ISourceGenerator
    {
        readonly Dictionary<string, Method> methods = (Enum.GetValues(typeof(Method)) as Method[]).ToDictionary(x => x.ToString());

        public void Execute(GeneratorExecutionContext context)
        {
            var addedTreePath = "SpanLinq";

            var syntaxReciever = (SyntaxReciever)context.SyntaxReceiver!;
            var toInvestigate = syntaxReciever.ToInvestigate;
            var compilation = context.Compilation;
            var span = compilation.GetTypeByMetadataName(typeof(Span<>).FullName);
            var readOnlySpan = compilation.GetTypeByMetadataName(typeof(ReadOnlySpan<>).FullName);
            if (span is null || readOnlySpan is null)
                return;
            var source = "";

            var generated = new HashSet<(string sourceType, Method method)>();

            while (toInvestigate.Count > 0)
            {
                var addedClass = compilation.GetTypeByMetadataName("System.Linq.SpanLinq");
                var ourTypes = addedClass?.GetTypeMembers() ?? ImmutableArray<INamedTypeSymbol>.Empty;
#pragma warning disable RS1024 // Compare symbols correctly
                var toPotentiallyGenerateLinqFor = new HashSet<INamedTypeSymbol>(ourTypes, SymbolEqualityComparer.Default) { span, readOnlySpan };
#pragma warning restore RS1024 // Compare symbols correctly

                var updatedToInvestigate = toInvestigate.Where(x =>
                {
                    var semanticModel = compilation.GetSemanticModel(x.syntax.SyntaxTree);
                    if (semanticModel.GetSymbolInfo(x.syntax).Symbol is not null)
                    {
                        //Currently binds, so don't generate method for. Return true as fixing a call earlier in the chain can stop this binding.
                        return true;
                    }

                    if (semanticModel.GetTypeInfo(x.syntax.Expression).Type is INamedTypeSymbol { OriginalDefinition: var type }
                        && toPotentiallyGenerateLinqFor.Contains(type))
                    {
                        Generate(ref source, type, x.method, span, readOnlySpan, generated);
                        return false;
                    }

                    return true;
                }).ToList();

                if (updatedToInvestigate.Count == toInvestigate.Count || updatedToInvestigate.Count == 0)
                {
                    break;
                }

                toInvestigate = updatedToInvestigate;

                compilation = context.Compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(CreateFullSourceText(source), syntaxReciever.ParseOptions, path: addedTreePath));
            }

            if (source != "")
            {
                context.AddSource("SpanLinq.cs", SourceText.From(CreateFullSourceText(source), Encoding.UTF8));
            }
        }

        private static string CreateFullSourceText(string generatedTypes)
        {
            return 
$@"#pragma warning disable CS8019

using System.ComponentModel;
using System.Collections.Generic;

namespace System.Linq
{{
    internal static class SpanLinq
    {{{generatedTypes}    }}
}}";
        }

        private void Generate(ref string doc, INamedTypeSymbol type, Method method, INamedTypeSymbol span, INamedTypeSymbol readOnlySpan, HashSet<(string sourceType, Method method)> generated)
        {
            if (!generated.Add((type.Name, method)))
            {
                return;
            }

            var isSpan = type.Equals(span, SymbolEqualityComparer.Default);
            var isReadOnlySpan = type.Equals(readOnlySpan, SymbolEqualityComparer.Default);
            var sourceName = type.Name;
            var hasLength = type.GetMembers().Any(x => x is IPropertySymbol { Name: "Length" });
            switch (method)
            {
                case Select:
                    {
                        var sourceTypeParameters = type.TypeParameters.Length == 1
                            ? new List<string> { "TSource" }
                            : type.TypeParameters.Select(x => sourceName + x.Name).ToList();
                        var sourceTypeParametersString = string.Join(", ", sourceTypeParameters);
                        var sourceResult = sourceTypeParameters.Last();
                        var fullSourceName = $"{sourceName}<{sourceTypeParametersString}>";
                        var resultName = "Select" + (isReadOnlySpan ? "Span" : sourceName);
                        var fullResultName = $"{resultName}<{sourceTypeParametersString}, TResult>";
                        var funcName = $"Func<{sourceResult}, TResult>";

                        if (isSpan)
                        {
                            Generate(ref doc, readOnlySpan, method, span, readOnlySpan, generated);

                            doc += $@"
        public static {fullResultName} Select<{sourceTypeParametersString}, TResult>(this {fullSourceName} source, {funcName} selector)
        {{
            return ((ReadOnlySpan<{sourceResult}>)source).Select(selector);
        }}";
                        }
                        else
                        {
                            doc += $@"
        public static {fullResultName} Select<{sourceTypeParametersString}, TResult>(this {fullSourceName} source, {funcName} selector)
        {{
            return new {fullResultName}(source, selector);
        }}

        public ref struct {fullResultName}
        {{
            private {fullSourceName} source;
            private {funcName} selector;

            public {resultName}({fullSourceName} source, {funcName} selector)
            {{
                this.source = source;
                this.selector = selector;
            }}
            {(hasLength ? $@"
            public int Length => source.Length;

            [EditorBrowsable(EditorBrowsableState.Never)]
            internal void Slice(int start, int length)
            {{
                {(isReadOnlySpan ? "source = " : "")}source.Slice(start, length);
            }}
" : "")}
            public Enumerator GetEnumerator()
            {{
                return new Enumerator(this);
            }}

            public ref struct Enumerator
            {{
                private {funcName} selector;
                private {fullSourceName}.Enumerator enumerator;

                public Enumerator({fullResultName} outer)
                {{
                    selector = outer.selector;
                    enumerator = outer.source.GetEnumerator();
                    Current = default;
                }}

                public TResult Current {{ get; private set; }}

                public bool MoveNext()
                {{
                    if (!enumerator.MoveNext())
                    {{
                        return false;
                    }}
                    Current = selector(enumerator.Current);
                    return true;
                }}
            }}
        }}";
                        }
                    }
                    break;

                case Where:
                    {
                        var sourceTypeParameters = type.TypeParameters.Select(x => x.Name).ToList();
                        var sourceTypeParametersString = string.Join(", ", sourceTypeParameters);
                        var sourceResult = sourceTypeParameters.Last();
                        var fullSourceName = $"{sourceName}<{sourceTypeParametersString}>";
                        var resultName = "Where" + (isReadOnlySpan ? "Span" : sourceName);
                        var fullResultName = $"{resultName}<{sourceTypeParametersString}>";
                        var funcName = $"Func<{sourceResult}, bool>";
                        if (isSpan)
                        {
                            Generate(ref doc, readOnlySpan, method, span, readOnlySpan, generated);

                            doc += $@"
        public static {fullResultName} Where<{sourceTypeParametersString}>(this {fullSourceName} source, {funcName} predicate)
        {{
            return ((ReadOnlySpan<{sourceResult}>)source).Where(predicate);
        }}";
                        }
                        else
                        {
                            doc += $@"
        public static {fullResultName} Where<{sourceTypeParametersString}>(this {fullSourceName} source, {funcName} predicate)
        {{
            return new {fullResultName}(source, predicate);
        }}

        public ref struct {fullResultName}
        {{
            private {fullSourceName} source;
            private {funcName} predicate;

            public {resultName}({fullSourceName} source, {funcName} predicate)
            {{
                this.source = source;
                this.predicate = predicate;
            }}

            public Enumerator GetEnumerator()
            {{
                return new Enumerator(this);
            }}

            public ref struct Enumerator
            {{
                private {funcName} predicate;
                private {fullSourceName}.Enumerator enumerator;

                public Enumerator({fullResultName} outer)
                {{
                    predicate = outer.predicate;
                    enumerator = outer.source.GetEnumerator();
                }}

                public {sourceResult} Current => enumerator.Current;

                public bool MoveNext()
                {{
                    while (enumerator.MoveNext())
                    {{
                        if (predicate(enumerator.Current))
                        {{
                            return true;
                        }}
                    }}
                    return false;
                }}
            }}
        }}";
                        }
                    }
                    break;

                case ToList:
                    {
                        var sourceTypeParameters = type.TypeParameters.Select(x => x.Name).ToList();
                        var sourceTypeParametersString = string.Join(", ", sourceTypeParameters);
                        var sourceResult = sourceTypeParameters.Last();
                        var fullSourceName = $"{sourceName}<{sourceTypeParametersString}>";
                        if (isSpan)
                        {
                            Generate(ref doc, readOnlySpan, method, span, readOnlySpan, generated);

                            doc += $@"
        public static System.Collections.Generic.List<{sourceResult}> ToList<{sourceTypeParametersString}>(this {fullSourceName} source)
        {{
            return ((ReadOnlySpan<{sourceResult}>)source).ToList();
        }}";
                        }
                        else
                        {
                            doc += $@"
        public static System.Collections.Generic.List<{sourceResult}> ToList<{sourceTypeParametersString}>(this {fullSourceName} source)
        {{
            var list = new System.Collections.Generic.List<{sourceResult}>({(hasLength ? "source.Length" : "")});
            foreach (var item in source)
            {{
                list.Add(item);
            }}
            return list;
        }}";
                        }
                    }
                    break;

                case ToArray:
                    {
                        var sourceTypeParameters = type.TypeParameters.Select(x => x.Name).ToList();
                        var sourceTypeParametersString = string.Join(", ", sourceTypeParameters);
                        var sourceResult = sourceTypeParameters.Last();
                        var fullSourceName = $"{sourceName}<{sourceTypeParametersString}>";
                        if (isSpan)
                        {
                            Generate(ref doc, readOnlySpan, method, span, readOnlySpan, generated);

                            doc += $@"
        public static {sourceResult}[] ToArray<{sourceTypeParametersString}>(this {fullSourceName} source)
        {{
            return ((ReadOnlySpan<{sourceResult}>)source).ToArray();
        }}";
                        }
                        else if (hasLength)
                        {
                            doc += $@"
        public static {sourceResult}[] ToArray<{sourceTypeParametersString}>(this {fullSourceName} source)
        {{
            var array = new {sourceResult}[source.Length];
            int i = 0;
            foreach (var item in source)
            {{
                array[i] = item;
                i++;
            }}
            return array;
        }}";
                        }
                        else
                        {
                            doc += $@"
        public static {sourceResult}[] ToArray<{sourceTypeParametersString}>(this {fullSourceName} source)
        {{
            var list = new System.Collections.Generic.List<{sourceResult}>();
            foreach (var item in source)
            {{
                list.Add(item);
            }}
            return list.ToArray();
        }}";
                        }
                    }
                    break;

                case Take:
                    {
                        var sourceTypeParameters = type.TypeParameters.Select(x => x.Name).ToList();
                        var sourceTypeParametersString = string.Join(", ", sourceTypeParameters);
                        var sourceResult = sourceTypeParameters.Last();
                        var fullSourceName = $"{sourceName}<{sourceTypeParametersString}>";

                        if (hasLength)
                        {
                            doc += $@"
        public static {fullSourceName} Take<{sourceTypeParametersString}>(this {fullSourceName} source, int count)
        {{
            if (count < source.Length)
            {{
                {((isSpan || isReadOnlySpan) ? "source = " : "")}source.Slice(0, count);
            }}
            return source;
        }}";
                        }
                        else
                        {
                            var resultName = "Take" + (isReadOnlySpan ? "Span" : sourceName);
                            var fullResultName = $"{resultName}<{sourceTypeParametersString}>";

                            doc += $@"
        public static {fullResultName} Take<{sourceTypeParametersString}>(this {fullSourceName} source, int count)
        {{
            return new {fullResultName}(source, count);
        }}

        public ref struct {fullResultName}
        {{
            private {fullSourceName} source;
            private int count;

            public {resultName}({fullSourceName} source, int count)
            {{
                this.source = source;
                this.count = count;
            }}

            public Enumerator GetEnumerator()
            {{
                return new Enumerator(this);
            }}

            public ref struct Enumerator
            {{
                private int remaining;
                private {fullSourceName}.Enumerator enumerator;

                public Enumerator({fullResultName} outer)
                {{
                    remaining = outer.count;
                    enumerator = outer.source.GetEnumerator();
                }}

                public {sourceResult} Current => enumerator.Current;

                public bool MoveNext()
                {{
                    remaining--;
                    return remaining >= 0 && enumerator.MoveNext();
                }}
            }}
        }}";
                        }
                    }
                    break;

                case Skip:
                    {
                        var sourceTypeParameters = type.TypeParameters.Select(x => x.Name).ToList();
                        var sourceTypeParametersString = string.Join(", ", sourceTypeParameters);
                        var sourceResult = sourceTypeParameters.Last();
                        var fullSourceName = $"{sourceName}<{sourceTypeParametersString}>";

                        if (hasLength)
                        {
                            doc += $@"
        public static {fullSourceName} Skip<{sourceTypeParametersString}>(this {fullSourceName} source, int count)
        {{
            if (count < source.Length)
            {{
                {((isSpan || isReadOnlySpan) ? "source = " : "")}source.Slice(count, source.Length - count);
            }}
            else
            {{
                {((isSpan || isReadOnlySpan) ? "source = " : "")}source.Slice(0, 0);
            }}
            return source;
        }}";
                        }
                        else
                        {
                            var resultName = "Skip" + (isReadOnlySpan ? "Span" : sourceName);
                            var fullResultName = $"{resultName}<{sourceTypeParametersString}>";

                            doc += $@"
        public static {fullResultName} Skip<{sourceTypeParametersString}>(this {fullSourceName} source, int count)
        {{
            return new {fullResultName}(source, count);
        }}

        public ref struct {fullResultName}
        {{
            private {fullSourceName} source;
            private int count;

            public {resultName}({fullSourceName} source, int count)
            {{
                this.source = source;
                this.count = count;
            }}

            public Enumerator GetEnumerator()
            {{
                return new Enumerator(this);
            }}

            public ref struct Enumerator
            {{
                private int remaining;
                private {fullSourceName}.Enumerator enumerator;

                public Enumerator({fullResultName} outer)
                {{
                    remaining = outer.count;
                    enumerator = outer.source.GetEnumerator();
                }}

                public {sourceResult} Current => enumerator.Current;

                public bool MoveNext()
                {{
                    while(remaining > 0)
                    {{
                        if (!enumerator.MoveNext())
                            return false;
                        remaining--;
                    }}
                    return enumerator.MoveNext();
                }}
            }}
        }}";
                        }
                    }
                    break;
            }

            doc += @"
";
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReciever(methods));
        }
    }
}
