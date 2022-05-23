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

    [Fact]
    public static void EventSubscriptions_CanBeAsserted()
    {
        var substitute = Gen.Substitute<IEvent>().Create();

        void Handler()
        {
        }

        substitute.Object.SomeEvent += Handler;

        substitute.Received.SomeEvent.Add((Action)Handler).Once();
        substitute.Received.SomeEvent.Remove((Action)Handler).Never();
        
        substitute.Object.SomeEvent -= Handler;
        
        substitute.Received.SomeEvent.Remove((Action)Handler).Once();
    }
}
