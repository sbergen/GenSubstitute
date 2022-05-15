using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using GenSubstitute.SourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenSubstitute.Tests;

internal static class GeneratorUtility
{
    public static void AssertNoInspections(string inputCode)
    {
        var compilation = inputCode.CreateCompilation();
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
}
