using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenSubstitute.SourceGenerator
{
    // Model this to be compatible with incremental generators later
    internal static class SyntaxFilter
    {
        public static bool IsSubstituteCall(SyntaxNode node) => node is
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
