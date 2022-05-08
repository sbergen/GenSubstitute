using System.Text;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator.Utilities
{
    internal static class SymbolExtensions
    {
        // See https://github.com/dotnet/roslyn/issues/1891
        // This implementation was originally copied from here, but modified:
        // https://stackoverflow.com/questions/27105909/get-fully-qualified-metadata-name-in-roslyn
        public static string FullyQualifiedMetadataName(this INamedTypeSymbol symbol)
        {
            var builder = new StringBuilder(symbol.MetadataName);

            static bool IsRootNamespace(ISymbol symbol) => symbol is INamespaceSymbol { IsGlobalNamespace: true };

            ISymbol previous = symbol;
            var current = symbol.ContainingSymbol;
            while (current != null && !IsRootNamespace(current))
            {
                if (current is ITypeSymbol && previous is ITypeSymbol)
                {
                    builder.Insert(0, '+');
                }
                else
                {
                    builder.Insert(0, '.');
                }

                builder.Insert(0, current.OriginalDefinition.MetadataName);

                previous = current;
                current = current.ContainingSymbol;
            }

            return builder.ToString();
        }
    }
}
