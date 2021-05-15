using Xunit;

namespace FatCat.Fakes.Tests.Primitives
{
	public class BoolFakeItem
	{
		[Fact]
		public void CanFakeABool() => Faker.Create<bool>();

		[Fact]
		public void CanFakeANullableBool() => Faker.Create<bool?>();
	}
}