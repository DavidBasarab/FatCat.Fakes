using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests.RandomTests
{
	public class RandomNumber
	{
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
		public void CanJustSendTheMaxValue()
		{
			var value = Faker.RandomInt(maxValue: 56);

			value.Should().BeInRange(int.MinValue, 56);
		}
	}
}