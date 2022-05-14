using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

public static class PropertyTests
{
    public interface IInterfaceWithProperties
    {
        public int Property { get; set; }
        public int ReadOnlyProperty { get; }
        public int WriteOnlyProperty { set; }
    }

    [Fact]
    public static void PropertyConfigurers_HaveCorrectType()
    {
        var builder = Gen.Substitute<IInterfaceWithProperties>().Build();

        builder.Configure.Property.Should().BeOfType<ConfiguredProperty<int>.ReadWrite>();
        builder.Configure.ReadOnlyProperty.Should().BeOfType<ConfiguredProperty<int>.ReadOnly>();
        builder.Configure.WriteOnlyProperty.Should().BeOfType<ConfiguredProperty<int>.WriteOnly>();
    }
    
    [Fact]
    public static void ReceivedProperties_HaveCorrectType()
    {
        var builder = Gen.Substitute<IInterfaceWithProperties>().Build();

        builder.Received.Property.Should().BeOfType<ReceivedPropertyCalls<int>.ReadWrite>();
        builder.Received.ReadOnlyProperty.Should().BeOfType<ReceivedPropertyCalls<int>.ReadOnly>();
        builder.Received.WriteOnlyProperty.Should().BeOfType<ReceivedPropertyCalls<int>.WriteOnly>();
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
    
    [Fact]
    public static void Properties_RetainTheirValue_WhenConfigured()
    {
        var builder = Gen.Substitute<IInterfaceWithProperties>().Build();
        
        builder.Configure.Property.RetainValue();
        
        builder.Object.Property = 42;
        builder.Object.Property.Should().Be(42);
    }
    
    [Fact]
    public static void ConfiguringGet_Throws_AfterRetaining()
    {
        var builder = Gen.Substitute<IInterfaceWithProperties>().Build();
        
        builder.Configure.Property.RetainValue();
        var configure = () => builder.Configure.Property.Get();
        configure.Should().Throw<InvalidPropertyConfigurationException>();
    }
    
    [Fact]
    public static void ConfiguringSet_Throws_AfterRetaining()
    {
        var builder = Gen.Substitute<IInterfaceWithProperties>().Build();
        
        builder.Configure.Property.RetainValue();
        var configure = () => builder.Configure.Property.Set(Arg<int>.Any);
        configure.Should().Throw<InvalidPropertyConfigurationException>();
    }
    
    [Fact]
    public static void Retaining_Throws_AfterConfiguringGet()
    {
        var builder = Gen.Substitute<IInterfaceWithProperties>().Build();
        
        builder.Configure.Property.Get();
        var retain = () => builder.Configure.Property.RetainValue();
        retain.Should().Throw<InvalidPropertyConfigurationException>();
    }
    
    [Fact]
    public static void Retaining_Throws_AfterConfiguringSet()
    {
        var builder = Gen.Substitute<IInterfaceWithProperties>().Build();
        
        builder.Configure.Property.Set(default);
        var retain = () => builder.Configure.Property.RetainValue();
        retain.Should().Throw<InvalidPropertyConfigurationException>();
    }
}
