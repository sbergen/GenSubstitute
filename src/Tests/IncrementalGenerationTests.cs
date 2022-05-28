using Microsoft.CodeAnalysis;
using Xunit;
using static GenSubstitute.Tests.GeneratorUtility;

namespace GenSubstitute.Tests;

public static class IncrementalGenerationTests
{
    [Fact]
    public static void AddingDuplicateSubstituteCall_DoesNotChangeModels() => AssertModelRunReasons(
        new[] { IncrementalStepRunReason.Unchanged },
        @"using GenSubstitute;
using System;

{0}",
        @"
var builder = Gen.Substitute<IDisposable>().Create();",
        @"
var builder = Gen.Substitute<IDisposable>().Create();
var builder2 = Gen.Substitute<IDisposable>().Create();");
    
    [Fact(Skip = "https://github.com/dotnet/roslyn/pull/61308")]
    public static void AddingNewSubstituteCall_GeneratesOnlyNewModel() => AssertModelRunReasons(
            new[] { IncrementalStepRunReason.Unchanged, IncrementalStepRunReason.New },
            @"using GenSubstitute;
using System;

{0}",
            @"
var builder = Gen.Substitute<IDisposable>().Create();",
            @"
var builder = Gen.Substitute<IDisposable>().Create();
var builder2 = Gen.Substitute<IComparable>().Create();");
}
