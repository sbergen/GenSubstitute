using System.Collections.Generic;

namespace GenSubstitute.SourceGenerator.Utilities
{
    // Potential to optimize, as this happens a lot!
    // Using a string builder and avoiding Linq was NOT faster in benchmarks and also allocated more memory
    // Using ImmutableArray.CreateRange was not clearly better either.
    internal static class ListStringUtils
    {
        public static string BuildList(IEnumerable<string> strings) => string.Join(", ", strings);
    }
}
