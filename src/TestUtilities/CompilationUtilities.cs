using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenSubstitute.TestUtilities;

public static class CompilationUtilities
{
    private static readonly string[] AssemblyNames =
    {
        "netstandard, Version=2.0.0.0",
        "System.Runtime, Version=6.0.0.0",
        "System.Private.CoreLib, Version=6.0.0.0",
    };

    private static readonly PortableExecutableReference[] References = AssemblyNames
        .Select(name => Assembly.Load(name).Location)
        .Append(typeof(Gen).Assembly.Location)
        .Select(file => MetadataReference.CreateFromFile(file))
        .ToArray();

    public static Compilation CreateCompilation(this string sourceCode) =>
        CSharpSyntaxTree.ParseText(sourceCode).CreateCompilation();

    public static Compilation CreateCompilation(this SyntaxTree syntaxTree)
    {
        var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication);
        
        return CSharpCompilation.Create(
            "GeneratorTestAssembly",
            new[] { syntaxTree },
            References,
            options);
    }
}
