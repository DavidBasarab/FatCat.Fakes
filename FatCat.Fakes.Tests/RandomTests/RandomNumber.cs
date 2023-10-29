using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests.RandomTests
{
    public class RandomNumber
    {
        [Fact]
        public void CanFakeALongInAGivenRange()
        {
            var value = Faker.RandomLong(14, 17);

            value.Should().BeInRange(14, 17);
        }

        [Fact]
        public void CanGetARandomLong()
        {
            var value = Faker.RandomLong();

            value.Should().BeInRange(long.MinValue, long.MaxValue);
        }

        [Fact]
        public void CanJustRestrictMaxOnLong()
        {
            var value = Faker.RandomLong(5);

            value.Should().BeInRange(long.MinValue, 5);
        }

        [Fact]
        public void CanFakeAnIntInAGivenRange()
        {
            var value = Faker.RandomInt(4, 7);

            value.Should().BeInRange(4, 7);
        }

        [Fact]
        public void CanGetARandomInt()
        {
            var value = Faker.RandomInt();

            value.Should().BeInRange(int.MinValue, int.MaxValue);
        }

        [Fact]
        public void CanJustRestrictMaxOnInt()
        {
            var value = Faker.RandomInt(56);

            value.Should().BeInRange(int.MinValue, 56);
        }
    }
}
