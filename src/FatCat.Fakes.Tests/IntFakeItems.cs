using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests
{
	public class IntFakeItems
	{
		[Fact]
		public void AnIntCanBeFaked()
		{
			var value = Faker.Create<int>();

			value.Should().BeInRange(int.MinValue, int.MaxValue);
		}

		[Fact]
		public void CanFakeByType()
		{
			var value = (int)Faker.Create(typeof(int));

			value.Should().BeInRange(int.MinValue, int.MaxValue);
		}

		[Fact]
		public void EachIntWillBeRandom()
		{
			var previousValue = 0;

			for (var i = 0; i < 13; i++)
			{
				var currentValue = Faker.Create<int>();

				currentValue.Should().NotBe(previousValue);

				previousValue = currentValue;
			}
		}
		
		[Fact]
		public void CanFakeANullableInt()
		{
			var value = Faker.Create<int?>();

			value.Should().BeInRange(int.MinValue, int.MaxValue);
		}
	}
}