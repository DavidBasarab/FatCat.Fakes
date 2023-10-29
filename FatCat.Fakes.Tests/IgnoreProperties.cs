using System;
using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests;

public class IgnoreProperties
{
    [Fact]
    public void CanIgnoreANonNullableType()
    {
        var item = Faker.Create<TestFakingItem>(propertiesToIgnore: i => i.SomeNumber);

        item.SomeString.Should().NotBeNullOrWhiteSpace();
        item.SomeNumber.Should().Be(default);

        item.FindMe.Should().NotBeNull();

        item.FindMe.First.Should().BeInRange(int.MinValue, int.MaxValue);
        item.FindMe.Second.Should().BeInRange(int.MinValue, int.MaxValue);
        item.FindMe.Date.Should().BeAfter(DateTime.MinValue);
    }

    [Fact]
    public void CanIgnoreASubTypeProperty()
    {
        var item = Faker.Create<TestFakingItem>(propertiesToIgnore: i => i.FindMe.SomeString);

        item.FindMe.First.Should().BeInRange(int.MinValue, int.MaxValue);
        item.FindMe.Second.Should().BeInRange(int.MinValue, int.MaxValue);
        item.FindMe.SomeString.Should().BeNull();
    }

    [Fact]
    public void CanIgnoreAType()
    {
        var item = Faker.Create<TestFakingItem>(propertiesToIgnore: i => i.FindMe);

        item.FindMe.Should().BeNull();
    }

    [Fact]
    public void CanIgnoreMultipleLevel()
    {
        var item = Faker.Create<TestFakingItem>(
            i => i.FindMe.DiveDive.SomeString,
            i => i.FindMe.DiveDive.GoLow.SomeString
        );

        item.FindMe.Should().NotBeNull();

        item.FindMe.DiveDive.GoLow.Should().NotBeNull();

        item.FindMe.DiveDive.SomeString.Should().BeNullOrWhiteSpace();

        item.FindMe.DiveDive.GoLow.SomeString.Should().BeNullOrWhiteSpace();

        item.FindMe.DiveDive.GoLow.SomeNumber.Should().NotBe(default);
    }

    [Fact]
    public void WillNotSetTheIgnoredProperty()
    {
        var item = Faker.Create<TestFakingItem>(propertiesToIgnore: i => i.SomeString);

        item.SomeString.Should().BeNullOrWhiteSpace();
        item.SomeNumber.Should().BeInRange(int.MinValue, int.MaxValue);

        item.FindMe.Should().NotBeNull();

        item.FindMe.First.Should().BeInRange(int.MinValue, int.MaxValue);
        item.FindMe.Second.Should().BeInRange(int.MinValue, int.MaxValue);
        item.FindMe.Date.Should().BeAfter(DateTime.MinValue);
    }

    public class DeeperClass
    {
        public LowestClass GoLow { get; set; }

        public string SomeString { get; set; }
    }

    public class LowestClass
    {
        public int SomeNumber { get; set; }

        public string SomeString { get; set; }
    }

    public class SubObject
    {
        public DateTime Date { get; set; }

        public DeeperClass DiveDive { get; set; }

        public int First { get; set; }

        public int Second { get; set; }

        public string SomeString { get; set; }
    }

    public class TestFakingItem
    {
        public SubObject FindMe { get; set; }

        public int SomeNumber { get; set; }

        public string SomeString { get; set; }
    }
}
