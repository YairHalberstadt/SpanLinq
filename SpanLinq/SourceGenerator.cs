using Microsoft.CodeAnalysis;
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
            if (compilation.GetTypeByMetadataName(typeof(Span<>).FullName) is not { } span)
                return;
            var generatedTypes = "";

            toInvestigate = toInvestigate.Where(x => compilation.GetSemanticModel(x.syntax.SyntaxTree).GetSymbolInfo(x.syntax).Symbol is null).ToList();

            while (toInvestigate.Count > 0)
            {
                var generated = new HashSet<(ITypeSymbol sourceType, Method method)>();

                var addedClass = compilation.GetTypeByMetadataName("System.Linq.SpanLinq");
                var ourTypes = addedClass?.GetTypeMembers() ?? ImmutableArray<INamedTypeSymbol>.Empty;
                var toPotentiallyGenerateLinqFor = new HashSet<INamedTypeSymbol>(ourTypes, SymbolEqualityComparer.Default) { span };

                var updatedToInvestigate = toInvestigate.Where(x =>
                {
                    if (compilation.GetSemanticModel(x.syntax.SyntaxTree).GetTypeInfo(x.syntax.Expression).Type is INamedTypeSymbol { OriginalDefinition: var type }
                        && toPotentiallyGenerateLinqFor.Contains(type))
                    {
                        if (generated.Add((type, x.method)))
                        {
                            if (generatedTypes != "")
                            {
                                generatedTypes += @"
";
                            }
                            Generate(ref generatedTypes, type, x.method);
                        }
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }).ToList();

                if (updatedToInvestigate.Count == toInvestigate.Count || updatedToInvestigate.Count == 0)
                {
                    break;
                }

                toInvestigate = updatedToInvestigate;

                compilation = context.Compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(CreateFullSourceText(generatedTypes), syntaxReciever.ParseOptions, path: addedTreePath));
            }

            if (generatedTypes != "")
            {
                context.AddSource("SpanLinq.cs", SourceText.From(CreateFullSourceText(generatedTypes), Encoding.UTF8));
            }
        }

        private static string CreateFullSourceText(string generatedTypes)
        {
            return 
$@"namespace System.Linq
{{
    internal static class SpanLinq
    {{{generatedTypes}
    }}
}}";
        }

        private void Generate(ref string doc, INamedTypeSymbol type, Method method)
        {
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
                        var resultName = "Select" + sourceName;
                        var fullResultName = $"{resultName}<{sourceTypeParametersString}, TResult>";
                        var funcName = $"Func<{sourceResult}, TResult>";

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
            {(hasLength ? @"
            public int Length => source.Length;
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
                    break;

                case Where:
                    {
                        var sourceTypeParameters = type.TypeParameters.Select(x => x.Name).ToList();
                        var sourceTypeParametersString = string.Join(", ", sourceTypeParameters);
                        var sourceResult = sourceTypeParameters.Last();
                        var fullSourceName = $"{sourceName}<{sourceTypeParametersString}>";
                        var resultName = "Where" + sourceName;
                        var fullResultName = $"{resultName}<{sourceTypeParametersString}>";
                        var funcName = $"Func<{sourceResult}, bool>";
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
                    break;

                case ToList:
                    {
                        var sourceTypeParameters = type.TypeParameters.Select(x => x.Name).ToList();
                        var sourceTypeParametersString = string.Join(", ", sourceTypeParameters);
                        var sourceResult = sourceTypeParameters.Last();
                        var fullSourceName = $"{sourceName}<{sourceTypeParametersString}>";
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
                    break;

                case ToArray:
                    {
                        var sourceTypeParameters = type.TypeParameters.Select(x => x.Name).ToList();
                        var sourceTypeParametersString = string.Join(", ", sourceTypeParameters);
                        var sourceResult = sourceTypeParameters.Last();
                        var fullSourceName = $"{sourceName}<{sourceTypeParametersString}>";
                        if (hasLength)
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
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReciever(methods));
        }
    }
}
