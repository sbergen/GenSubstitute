using System.Collections.Generic;

namespace GenSubstitute.SourceGenerator.Utilities
{
    /// <summary>
    /// Generates unique names for local variables
    /// </summary>
    public class LocalNameUniquifier
    {
        private readonly HashSet<string> _reserved;

        public LocalNameUniquifier(IEnumerable<string> reservedUniqueNames) => _reserved =
            new(reservedUniqueNames);

        public string GetUniqueLocalName(string name)
        {
            // We assume there aren't too many, so just brute-force this:
            var baseName = $"local_{name}";
            var candidate = baseName;
            var i = 0;
            while (_reserved.Contains(candidate))
            {
                candidate = $"{baseName}_{++i}";
            }
            
            _reserved.Add(candidate);
            return candidate;
        }
    }
}
