using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace GenSubstitute.Tests;

public static class AsyncMockTests
{
    public interface IAsyncInterface
    {
        Task<int> AsyncMethod();
    }
    
    [Fact]
    public static async Task AsyncMethod_CanBeMockedWithAsyncLambda()
    {
        var substitute = Gen.Substitute<IAsyncInterface>().Create();
        substitute.SetUp.AsyncMethod().As(async () =>
        {
            await Task.Delay(1);
            return 42;
        });

        var result = await substitute.Object.AsyncMethod();
        result.Should().Be(42);
    }
}
