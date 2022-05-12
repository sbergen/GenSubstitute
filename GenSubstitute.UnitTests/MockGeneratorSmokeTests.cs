using Xunit;
using static GenSubstitute.UnitTests.GeneratorUtility;

namespace GenSubstitute.UnitTests;

public static class MockGeneratorSmokeTests
{
    [Fact]
    public static void GenericsSmokeTest() => AssertNoInspections(@"
using GenSubstitute;

var builder = Gen.Substitute<IFoo<int>>().Build();
var builder2 = Gen.Substitute<IFoo<double>>().Build();

public interface IFoo<T>
{
    void TakesGenericArg(T val); 
    T ReturnsGenericArg();
    T2 HasGenericArgs<T2, T3>(T3 val);
}");

    [Fact]
    public static void ExternalTypeSmokeTest() => AssertNoInspections(@"
using GenSubstitute;
using System;

var externalBuilder = Gen.Substitute<IDisposable>().Build();");

    [Fact]
    public static void RefAndOutArgsSmokeTest() => AssertNoInspections(@"
using GenSubstitute;

var externalBuilder = Gen.Substitute<IRefAndOutArgs>().Build();

interface IRefAndOutArgs
{
    int TakesRefArg(double foo, ref int bar);
    bool TakesOutArg(string foo, out int bar);
}");
    
    [Fact]
    public static void DefaultArgsSmokeTest() => AssertNoInspections(@"
using GenSubstitute;
using System;

var externalBuilder = Gen.Substitute<IDefaultArgs>().Build();

interface IDefaultArgs
{
    void HasDefaultArgs(
        int i = 1,
        double d = 2.0,
        string s = ""foo"",
        TypeCode t = TypeCode.Boolean);
}");
    
    [Fact]
    public static void PropertiesSmokeTest() => AssertNoInspections(@"
using GenSubstitute;

var externalBuilder = Gen.Substitute<IProperties>().Build();

interface IProperties
{
    int GetOnlyProperty { get; }
    int SetOnlyProperty { set; }
    int GetAndSetProperty { get; set; }
}");
    
    [Fact]
    public static void NullabilitySmokeTest() => AssertNoInspections(@"
using GenSubstitute;

var externalBuilder = Gen.Substitute<INullables>().Build();

interface INullables
{
    int? ReturnsNullable();
    void TakesNullable(int? arg);
    int? NullableProperty { get; set; }
}");
}
