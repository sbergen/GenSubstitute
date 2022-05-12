namespace GenSubstitute.SourceGenerator.Utilities
{
    internal static class InternalName
    {
        // Double underscores shouldn't be used in user code,
        // and adding "gensub" (not used anywhere else) should be fairly safe. 
        public static string Make(string originalName) => $"gensub__{originalName}";
        
        // Slight optimization for two-part names
        public static string Make(string firstPart, string secondPart) => $"gensub__{firstPart}_{secondPart}";
    }
}
