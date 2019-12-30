using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests.Primitives
{
	public class IntFakeItems : PrimitiveTests<int>
	{
		[Fact]
		public void AnIntCanBeFaked()
		{
			var value = Faker.Create<int>();

			value.Should().BeInRange(int.MinValue, int.MaxValue);
		}

		[Fact]
		public void CanFakeANullableInt()
		{
			var value = Faker.Create<int?>();

			value.Should().BeInRange(int.MinValue, int.MaxValue);
		}

		[Fact]
		public void CanFakeByType()
		{
			var value = (int)Faker.Create(typeof(int));

			value.Should().BeInRange(int.MinValue, int.MaxValue);
		}
	}
}