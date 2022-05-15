using FluentAssertions;
using Xunit;

namespace GenSubstitute.UsageTests;

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
        var substitute = Gen.Substitute<IInterfaceWithProperties>().Create();

        substitute.SetUp.Property.Should().BeOfType<ConfiguredProperty<int>.ReadWrite>();
        substitute.SetUp.ReadOnlyProperty.Should().BeOfType<ConfiguredProperty<int>.ReadOnly>();
        substitute.SetUp.WriteOnlyProperty.Should().BeOfType<ConfiguredProperty<int>.WriteOnly>();
    }
    
    [Fact]
    public static void ReceivedProperties_HaveCorrectType()
    {
        var substitute = Gen.Substitute<IInterfaceWithProperties>().Create();

        substitute.Received.Property.Should().BeOfType<ReceivedPropertyCalls<int>.ReadWrite>();
        substitute.Received.ReadOnlyProperty.Should().BeOfType<ReceivedPropertyCalls<int>.ReadOnly>();
        substitute.Received.WriteOnlyProperty.Should().BeOfType<ReceivedPropertyCalls<int>.WriteOnly>();
    }

    [Fact]
    public static void PropertyReturnValues_CanBeMocked()
    {
        var substitute = Gen.Substitute<IInterfaceWithProperties>().Create();
        
        substitute.SetUp.Property.Get().Returns(42);
        substitute.Object.Property.Should().Be(42);
    }

    [Fact]
    public static void ReceivedPropertyGets_CanBeChecked()
    {
        var substitute = Gen.Substitute<IInterfaceWithProperties>().Create();
        substitute.Object.Property.Should().Be(0);
        substitute.Received.Property.Get().Count.Should().Be(1);
    }

    [Fact]
    public static void PropertySetters_CanBeConfigured()
    {
        var substitute = Gen.Substitute<IInterfaceWithProperties>().Create();

        int val = 0;
        substitute.SetUp.Property.Set(Arg.Any).As(v => val = v);
        substitute.Object.Property = 42;
        val.Should().Be(42);
    }
    
    [Fact]
    public static void CallsToPropertySetters_CanBeCheckedByArgValue()
    {
        var substitute = Gen.Substitute<IInterfaceWithProperties>().Create();
        
        substitute.Object.Property = 1;
        substitute.Object.Property = 2;
        substitute.Received.Property.Set(1).Count.Should().Be(1);
    }
    
    [Fact]
    public static void Properties_RetainTheirValue_WhenConfigured()
    {
        var substitute = Gen.Substitute<IInterfaceWithProperties>().Create();
        
        substitute.SetUp.Property.RetainValue();
        
        substitute.Object.Property = 42;
        substitute.Object.Property.Should().Be(42);
    }
    
    [Fact]
    public static void ConfiguringGet_Throws_AfterRetaining()
    {
        var substitute = Gen.Substitute<IInterfaceWithProperties>().Create();
        
        substitute.SetUp.Property.RetainValue();
        var configure = () => substitute.SetUp.Property.Get();
        configure.Should().Throw<InvalidPropertyConfigurationException>();
    }
    
    [Fact]
    public static void ConfiguringSet_Throws_AfterRetaining()
    {
        var substitute = Gen.Substitute<IInterfaceWithProperties>().Create();
        
        substitute.SetUp.Property.RetainValue();
        var configure = () => substitute.SetUp.Property.Set(Arg<int>.Any);
        configure.Should().Throw<InvalidPropertyConfigurationException>();
    }
    
    [Fact]
    public static void Retaining_Throws_AfterConfiguringGet()
    {
        var substitute = Gen.Substitute<IInterfaceWithProperties>().Create();
        
        substitute.SetUp.Property.Get();
        var retain = () => substitute.SetUp.Property.RetainValue();
        retain.Should().Throw<InvalidPropertyConfigurationException>();
    }
    
    [Fact]
    public static void Retaining_Throws_AfterConfiguringSet()
    {
        var substitute = Gen.Substitute<IInterfaceWithProperties>().Create();
        
        substitute.SetUp.Property.Set(default);
        var retain = () => substitute.SetUp.Property.RetainValue();
        retain.Should().Throw<InvalidPropertyConfigurationException>();
    }
}
