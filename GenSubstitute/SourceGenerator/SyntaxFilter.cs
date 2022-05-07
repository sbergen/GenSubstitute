using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenSubstitute.SourceGenerator
{
    internal static class SyntaxFilter
    {
        public static bool IsSubstituteCall(SyntaxNode node, CancellationToken ct) => node is
            InvocationExpressionSyntax
            {
                Expression: MemberAccessExpressionSyntax
                {
                    Expression: IdentifierNameSyntax
                    {
                        Identifier: { ValueText: nameof(Gen) }
                    },
                    Name: GenericNameSyntax
                    {
                        Identifier: { ValueText: nameof(Gen.Substitute) }
                    }
                },
            };
        
        public static TypeSyntax ExtractTypeFromSubstituteCall(SyntaxNode node)
        {
            return node is InvocationExpressionSyntax
            {
                Expression: MemberAccessExpressionSyntax
                {
                    Name: GenericNameSyntax
                    {
                        TypeArgumentList:
                        {
                            Arguments: { Count: 1 } typeArguments,
                        },
                    },
                },
            } ? typeArguments[0] : null;
        }
    }
}
