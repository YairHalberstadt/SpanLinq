using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

#nullable enable
namespace SpanLinq
{
    class SyntaxReciever : ISyntaxReceiver
    {
        private readonly Dictionary<string, Method> methods;

        public List<MemberAccessExpressionSyntax> ToInvestigate { get; } = new();

        public SyntaxReciever(Dictionary<string, Method> methods)
        {
            this.methods = methods;
        }

        public CSharpParseOptions ParseOptions { get; private set; } = null!;

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (ParseOptions is null)
            {
                ParseOptions = (CSharpParseOptions)syntaxNode.SyntaxTree.Options;
            }

            if (syntaxNode is MemberAccessExpressionSyntax { Name: var memberName } memberAccessExpression && methods.TryGetValue(memberName.Identifier.ValueText, out var method))
            {
                ToInvestigate.Add(memberAccessExpression);
            }
        }
    }
}
