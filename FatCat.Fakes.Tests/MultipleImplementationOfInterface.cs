using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests;

public class MultipleImplementationOfInterface
{
    [Fact]
    public void WillRandomlyPickAnImplementationOfAnInterface()
    {
        var firstFound = false;
        var secondFound = false;
        var thirdFound = false;

        for (var i = 0; i < 30; i++)
        {
            var item = Faker.Create<IMyInterface>();

            firstFound = firstFound || item is FirstImplementation;
            secondFound = secondFound || item is SecondImplementation;
            thirdFound = thirdFound || item is ThirdImplementation;
        }

        firstFound.Should().BeTrue();
        secondFound.Should().BeTrue();
        thirdFound.Should().BeTrue();
    }

    private class FirstImplementation : IMyInterface
    {
        public int SomeNumber { get; set; }

        public string SomeString { get; set; }
    }

    private interface IMyInterface
    {
        public int SomeNumber { get; set; }

        public string SomeString { get; set; }
    }

    private class SecondImplementation : IMyInterface
    {
        public int SomeNumber { get; set; }

        public string SomeString { get; set; }
    }

    private class ThirdImplementation : IMyInterface
    {
        public int SomeNumber { get; set; }

        public string SomeString { get; set; }
    }
}
