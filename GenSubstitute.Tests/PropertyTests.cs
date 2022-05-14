using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

public static class PropertyTests
{
    public interface IInterfaceWithProperties
    {
        public int Property { get; set; }
    }

    [Fact]
    public static void PropertyReturnValues_CanBeMocked()
    {
        var builder = Gen.Substitute<IInterfaceWithProperties>().Build();
        
        builder.Configure.Property.Get().Returns(42);
        builder.Object.Property.Should().Be(42);
    }

    [Fact]
    public static void ReceivedPropertyGets_CanBeChecked()
    {
        var builder = Gen.Substitute<IInterfaceWithProperties>().Build();
        builder.Object.Property.Should().Be(0);
        builder.Received.Property.Get().Count.Should().Be(1);
    }

    [Fact]
    public static void PropertySetters_CanBeConfigured()
    {
        var builder = Gen.Substitute<IInterfaceWithProperties>().Build();

        int val = 0;
        builder.Configure.Property.Set(Arg.Any).Configure(v => val = v);
        builder.Object.Property = 42;
        val.Should().Be(42);
    }
    
    [Fact]
    public static void CallsToPropertySetters_CanBeCheckedByArgValue()
    {
        var builder = Gen.Substitute<IInterfaceWithProperties>().Build();
        
        builder.Object.Property = 1;
        builder.Object.Property = 2;
        builder.Received.Property.Set(1).Count.Should().Be(1);
    }
    
    /*
    [Fact]
    public static void Properties_RetainTheirValue()
    {
        var builder = Gen.Substitute<IInterfaceWithProperties>().Build();
        builder.Object.Property = 42;
        builder.Object.Property.Should().Be(42);
    }
    */
}
