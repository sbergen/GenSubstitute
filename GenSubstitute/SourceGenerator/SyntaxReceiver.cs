using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenSubstitute.SourceGenerator
{
    internal class SyntaxReceiver : ISyntaxReceiver
    {
        private readonly List<InvocationExpressionSyntax> _generateCalls = new();
        public IReadOnlyList<InvocationExpressionSyntax> GenerateCalls => _generateCalls;

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (SyntaxFilter.IsSubstituteCall(syntaxNode))
            {
                _generateCalls.Add((InvocationExpressionSyntax)syntaxNode);
            }
        }
    }
}
