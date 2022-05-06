using System.Linq;
using System.Reflection;
using System.Text;
using FluentAssertions;
using GenSubstitute.SourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenSubstitute.Tests;

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
    
    public static SyntaxTree GenerateCode(string inputCode)
    {
        var compilation = CreateCompilation(inputCode);
        var driver = CSharpGeneratorDriver.Create(new MockGenerator());
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        var generatedCode = outputCompilation.SyntaxTrees
            .Single(c => c.FilePath.Contains(MockGenerator.OutputFileName));
        
        diagnostics.Should().BeEmpty();
        outputCompilation.GetDiagnostics().Should()
            .BeEmpty(
                "the combined code shouldn't have inspections:\n{0}\n\n{1}",
                AddLineNumbers(inputCode),
                AddLineNumbers(generatedCode));

        return generatedCode;
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

    private static string AddLineNumbers(SyntaxTree tree) => AddLineNumbers(tree.ToString());

    private static string AddLineNumbers(string code)
    {
        var builder = new StringBuilder();
        foreach (var (line, index) in code.Split("\n").Select((l, i) => (l, i + 1)))
        {
            builder.AppendLine($"{index:000}: {line}");
        }

        return builder.ToString();
    }
}
