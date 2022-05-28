using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using FluentAssertions.Execution;
using GenSubstitute.SourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenSubstitute.Tests;

internal static class GeneratorUtility
{
    public static void AssertNoDiagnostics(string inputCode)
    {
        var compilation = inputCode.CreateCompilation();
        var driver = CSharpGeneratorDriver.Create(new GenSubstituteGenerator());
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        using var assertionScope = new AssertionScope();
        assertionScope.AddReportable("source", () => BuildSourceOutput(outputCompilation.SyntaxTrees));
        
        diagnostics.Should().BeEmpty("generators should not produce diagnostics");
        outputCompilation.GetDiagnostics().Should().BeEmpty("the combined code should not produce diagnostics");
    }

    private static string BuildSourceOutput(IEnumerable<SyntaxTree> syntaxTrees)
    {
        var builder = new StringBuilder();

        foreach (var syntaxTree in syntaxTrees)
        {
            if (!string.IsNullOrEmpty(syntaxTree.FilePath))
            {
                builder.AppendLine(syntaxTree.FilePath);
            }
            
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
