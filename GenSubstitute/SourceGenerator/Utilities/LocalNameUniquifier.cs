using System.Collections.Generic;

namespace GenSubstitute.SourceGenerator.Utilities
{
    /// <summary>
    /// Generates unique names for local variables.
    /// Since I seem to need unique names in more contexts than I thought,
    /// I'll retire this class for now, but keep it around in case I find I need it again.
    /// Using <see cref="InternalName"/> instead.
    /// </summary>
    internal class LocalNameUniquifier
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
