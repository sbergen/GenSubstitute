using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenSubstitute.SourceGenerator
{
    internal class SubstituteSyntaxReceiver : ISyntaxReceiver
    {
        public readonly List<TypeSyntax> SubstituteCalls = new();
        
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (SyntaxFilter.IsSubstituteCall(syntaxNode, CancellationToken.None) &&
                SyntaxFilter.ExtractTypeFromSubstituteCall(syntaxNode) is { } typeSyntax)
            {
                SubstituteCalls.Add(typeSyntax);
            }
        }
    }
}
