using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentAssertions;
using GenSubstitute.SourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenSubstitute.UnitTests;

internal static class GeneratorUtility
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
    
    public static void AssertNoInspections(string inputCode)
    {
        var compilation = CreateCompilation(inputCode);
        var driver = CSharpGeneratorDriver.Create(new GenSubstituteGenerator());
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        diagnostics.Should().BeEmpty();
        outputCompilation.GetDiagnostics().Should()
            .BeEmpty(
                $"the combined code shouldn't have inspections:\n{BuildSourceOutput(outputCompilation.SyntaxTrees)}");
    }

    private static string BuildSourceOutput(IEnumerable<SyntaxTree> syntaxTrees)
    {
        var builder = new StringBuilder();

        foreach (var syntaxTree in syntaxTrees)
        {
            builder.AppendLine(syntaxTree.FilePath);
            builder.AppendLine("---");

            foreach (var (line, index) in syntaxTree
                .ToString()
                .Split("\n")
                .Select((l, i) => (l, i + 1)))
            {
                builder.AppendLine($"{index:000}: {line}");
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }

    private static Compilation CreateCompilation(string sourceCode)
    {
        var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication);
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        return CSharpCompilation.Create(
            "GeneratorTestAssembly",
            new[] { syntaxTree },
            References,
            options);
    }
}
