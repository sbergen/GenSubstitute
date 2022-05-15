using Xunit;
using static GenSubstitute.Tests.GeneratorUtility;

namespace GenSubstitute.Tests;

public static class MockGeneratorSmokeTests
{
    [Fact]
    public static void GenericsSmokeTest() => AssertNoInspections(@"
using GenSubstitute;

var builder = Gen.Substitute<IFoo<int>>().Create();
var builder2 = Gen.Substitute<IFoo<double>>().Create();

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

var externalBuilder = Gen.Substitute<IDisposable>().Create();");

    [Fact]
    public static void RefAndOutArgsSmokeTest() => AssertNoInspections(@"
using GenSubstitute;

var externalBuilder = Gen.Substitute<IRefAndOutArgs>().Create();

interface IRefAndOutArgs
{
    int TakesRefArg(double foo, ref int bar);
    bool TakesOutArg(string foo, out int bar);
}");
    
    [Fact]
    public static void DefaultArgsSmokeTest() => AssertNoInspections(@"
using GenSubstitute;
using System;

var externalBuilder = Gen.Substitute<IDefaultArgs>().Create();

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

var externalBuilder = Gen.Substitute<IProperties>().Create();

interface IProperties
{
    int GetOnlyProperty { get; }
    int SetOnlyProperty { set; }
    int GetAndSetProperty { get; set; }
}");
    
    [Fact]
    public static void NullabilitySmokeTest() => AssertNoInspections(@"
#nullable enable
using GenSubstitute;

var externalBuilder = Gen.Substitute<INullables>().Create();

interface INullables
{
    int? ReturnsNullableValueType();
    object? ReturnsNullableReferenceType();
    void TakesNullableValueType(int? arg);
    void TakesNullableReferenceType(object? arg);
    int? NullableProperty { get; set; }
}");
}
