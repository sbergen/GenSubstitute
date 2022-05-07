using System.Collections.Generic;

namespace GenSubstitute.SourceGenerator.Utilities
{
    internal static class ListStringBuilder
    {
        // Potential to optimize, as this happens a lot!
        public static string BuildList(IEnumerable<string> strings) => string.Join(", ", strings);
    }   
}
