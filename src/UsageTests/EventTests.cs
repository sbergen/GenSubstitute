using System;
using FluentAssertions;
using Xunit;

namespace GenSubstitute.UsageTests;

public static class EventTests
{
    public interface IEvent
    {
        event Action SomeEvent;
    }
    
    [Fact]
    public static void Event_CanBeRaised()
    {
        var substitute = Gen.Substitute<IEvent>().Create();
        var wasRaised = false;
        substitute.Object.SomeEvent += () => wasRaised = true;
        
        substitute.Raise.SomeEvent();

        wasRaised.Should().BeTrue();
    }
}
