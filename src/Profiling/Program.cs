using GenSubstitute.SourceGenerator.Models;
using GenSubstitute.SourceGenerator.SourceBuilders;
using GenSubstitute.TestUtilities;
using JetBrains.Profiler.Api;

var source = @"
using System;

interface IBaseInterface : IDisposable
{
    int BaseProperty { get; }
    string BaseMethod(int foo, string bar, int[] array);
    event Action BaseEvent;
}

interface ITestInterface : IBaseInterface
{
    int DerivedProperty { get; }
    string DerivedMethod(int foo, string bar, int[] array);
    event Action DerivedEvent;
}";

var compilation = source.CreateCompilation();

var typeSymbol = compilation
    .GetSemanticModel(compilation.SyntaxTrees.First())
    .Compilation
    .GetTypeByMetadataName("ITestInterface");

var model1 = new TypeModel(typeSymbol!);

// Profile the second run, after type information has been cached,
// to get a better picture of our code's performance.
// Profiling the above call is valid also, but produces different data.
// It's hard to know what data is cached already when working in a real environment.
MeasureProfiler.StartCollectingData();

int counter = 0;

for (int i = 0; i < 100000; ++i)
{
    counter += MockBuilder.BuildMock(new TypeModel(typeSymbol!)).Length;
}

MeasureProfiler.SaveData();

Console.WriteLine(counter);
