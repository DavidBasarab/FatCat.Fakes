using System;
using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests.Primitives
{
    public class FakeDateTime : PrimitiveTests<DateTime>
    {
        [Fact]
        public void ANullableDateTimeCanBeFaked()
        {
            var value = Faker.Create<DateTime?>();

            value.Should().BeAfter(DateTime.MinValue);
            value.Should().BeBefore(DateTime.MaxValue);
        }

        [Fact]
        public void CanCreateAFakeDateTimeFromStaticMethod()
        {
            var value = Faker.RandomDateTime();

            value.Should().BeAfter(DateTime.MinValue);

            value.Should().BeBefore(DateTime.MaxValue);
        }

        [Fact]
        public void CanFakeADateTime()
        {
            var value = Faker.Create<DateTime>();

            value.Should().BeAfter(DateTime.MinValue);
            value.Should().BeBefore(DateTime.MaxValue);
        }
    }
}
