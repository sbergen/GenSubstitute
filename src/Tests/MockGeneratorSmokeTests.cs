using Xunit;
using static GenSubstitute.Tests.GeneratorUtility;

namespace GenSubstitute.Tests;

public static class MockGeneratorSmokeTests
{
    [Fact]
    public static void InterfaceInheritanceSmokeTest() => AssertNoDiagnostics(@"
using GenSubstitute;

var builder = Gen.Substitute<IDerived>().Create();

public interface IBase
{
    void BaseMethod();
}

public interface IDerived : IBase
{
    void DerivedMethod();
}");
    
    [Fact]
    public static void GenericsSmokeTest() => AssertNoDiagnostics(@"
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
    public static void ExternalTypeSmokeTest() => AssertNoDiagnostics(@"
using GenSubstitute;
using System;

var builder = Gen.Substitute<IDisposable>().Create();");

    [Fact]
    public static void RefAndOutArgsSmokeTest() => AssertNoDiagnostics(@"
using GenSubstitute;

var builder = Gen.Substitute<IRefAndOutArgs>().Create();

interface IRefAndOutArgs
{
    int TakesRefArg(double foo, ref int bar);
    bool TakesOutArg(string foo, out int bar);
}");
    
    [Fact]
    public static void DefaultArgsSmokeTest() => AssertNoDiagnostics(@"
using GenSubstitute;
using System;

var builder = Gen.Substitute<IDefaultArgs>().Create();

interface IDefaultArgs
{
    void HasDefaultArgs(
        int i = 1,
        double d = 2.0,
        string s = ""foo"",
        TypeCode t = TypeCode.Boolean);
}");
    
    [Fact]
    public static void PropertiesSmokeTest() => AssertNoDiagnostics(@"
using GenSubstitute;

var builder = Gen.Substitute<IProperties>().Create();

interface IProperties
{
    int GetOnlyProperty { get; }
    int SetOnlyProperty { set; }
    int GetAndSetProperty { get; set; }
}");
    
    [Fact]
    public static void NullabilitySmokeTest() => AssertNoDiagnostics(@"
#nullable enable
using GenSubstitute;

var builder = Gen.Substitute<INullables>().Create();

interface INullables
{
    int? ReturnsNullableValueType();
    object? ReturnsNullableReferenceType();
    void TakesNullableValueType(int? arg);
    void TakesNullableReferenceType(object? arg);
    int? NullableProperty { get; set; }
    object? NullableReferenceTypeProperty { get; set; }
}");
    
    [Fact]
    public static void EventsSmokeTest() => AssertNoDiagnostics(@"
#nullable enable
using GenSubstitute;
using System;

var builder = Gen.Substitute<IEvents>().Create();

interface IEvents
{
    event EventHandler TypicalEvent;
    event EventHandler? TypicalEvent2;
    event Func<int> UntypicalEvent;
    event Func<int?> UntypicalEvent2;
    event Func<object?>? UntypicalEvent3;
}");
}
