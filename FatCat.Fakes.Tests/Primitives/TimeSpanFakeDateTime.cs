using System;
using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests.Primitives
{
    public class TimeSpanFakeDateTime : PrimitiveTests<TimeSpan>
    {
        [Fact]
        public void CanFakeANullableTimeSpan()
        {
            var value = Faker.Create<TimeSpan?>();

            value.Should().BeGreaterThan(TimeSpan.MinValue);
            value.Should().BeLessThan(TimeSpan.MaxValue);
        }

        [Fact]
        public void CanFakeATimeSpan()
        {
            var value = Faker.Create<TimeSpan>();

            value.Should().BeGreaterThan(TimeSpan.MinValue);
            value.Should().BeLessThan(TimeSpan.MaxValue);
        }
    }
}
