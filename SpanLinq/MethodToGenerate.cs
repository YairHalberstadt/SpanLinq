using System.Linq;
using Microsoft.CodeAnalysis;
using static SpanLinq.Method;

namespace SpanLinq
{
    internal record GeneratedType(string Name, bool IsSpan, bool IsReadOnlySpan, bool HasLength, string[] TypeParameters);

    internal record MethodTogenerate(GeneratedType RecieverType, Method Method)
    {
        private GeneratedType? resultType;

        public GeneratedType? ResultType
        {
            get
            {
                if (resultType is not null)
                {
                    return resultType;
                }

                if (Method is not (Skip or Take or Select or Where or Reverse))
                {
                    return null;
                }

                var isSpan = RecieverType.IsSpan && Method is Skip or Take;
                var isReadOnlySpan = RecieverType.IsReadOnlySpan && Method is Skip or Take;
                var hasLength = RecieverType.HasLength && Method is not Where;
                var typeName = (Method is Skip or Take && hasLength)
                    ? RecieverType.Name
                    : Method.ToString() + (RecieverType.IsReadOnlySpan ? "Span" : RecieverType.Name);

                resultType = new(
                    Name: typeName,
                    IsSpan: isSpan,
                    IsReadOnlySpan: isReadOnlySpan,
                    HasLength: hasLength,
                    TypeParameters: Method switch
                    {
                        Select => SourceTypeParametersToUse.Append("TResult").ToArray(),
                        _ => SourceTypeParametersToUse,
                    }
                );

                return resultType;
            }
        }

        public string[] SourceTypeParametersToUse => Method switch
        {
            Select => RecieverType.TypeParameters.Length == 1
                ? new string[] { "TSource" }
                : RecieverType.TypeParameters.Select(x => RecieverType.Name + x).ToArray(),
            _ => RecieverType.TypeParameters,
        };

        public static MethodTogenerate FromTypeSymbolReciever(INamedTypeSymbol type, Method method, INamedTypeSymbol span, INamedTypeSymbol readOnlySpan)
        {
            return new(
                RecieverType: new(
                    Name: type.Name,
                    IsSpan: type.Equals(span, SymbolEqualityComparer.Default),
                    IsReadOnlySpan: type.Equals(readOnlySpan, SymbolEqualityComparer.Default),
                    HasLength: type.GetMembers().Any(x => x is IPropertySymbol { Name: "Length" }),
                    type.TypeParameters.Select(x => x.Name).ToArray()),
                Method: method
            );
        }
    }
}