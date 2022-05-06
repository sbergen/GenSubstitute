using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace GenSubstitute.Tests;

internal static class GeneratorUtility
{
    private static readonly string[] AssemblyNames =
    {
        "netstandard, Version=2.0.0.0",
    };

    private static readonly PortableExecutableReference[] References = AssemblyNames
        .Select(name => Assembly.Load(name).Location)
        .Append(typeof(Gen).Assembly.Location)
        .Select(file => MetadataReference.CreateFromFile(file))
        .ToArray();
    
    public static string GenerateCode(string inputCode)
    {
        var compilation = CreateCompilation(inputCode);
        var driver = CSharpGeneratorDriver.Create(new MockGenerator());
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);
        
        Assert.True(diagnostics.IsEmpty);
        
        return outputCompilation.SyntaxTrees
            .Single(c => c.FilePath.Contains(MockGenerator.OutputFileName))
            .ToString();
    }

    private static Compilation CreateCompilation(string sourceCode)
    {
        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        return CSharpCompilation.Create(
            "GeneratorTestAssembly",
            new[] { syntaxTree },
            References,
            options);
    }
}
