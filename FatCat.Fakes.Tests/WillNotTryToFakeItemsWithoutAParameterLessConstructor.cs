using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests
{
	public class WillNotTryToFakeItemsWithoutAParameterLessConstructor
	{
		private class ItemWithStuffInConstructor
		{
			public string SomeData { get; }

			public ItemWithStuffInConstructor(string someData) => SomeData = someData;
		}
		
		private class ItemToFake
		{
			public ItemWithStuffInConstructor ShouldNotBeFaked { get; set; }

			public int SomeNumber { get; set; }
		}
		
		[Fact]
		public void WillNotPopulateItemsWithoutAParameterLessConstructor()
		{
			var item = Faker.Create<ItemToFake>();

			item.ShouldNotBeFaked.Should().BeNull();
			item.SomeNumber.Should().BeInRange(int.MinValue, int.MaxValue);
		}
		
		[Fact]
		public void WillReturnNullIfTypeGivenDoesNotHaveAParameterLessConstructor()
		{
			var item = Faker.Create<ItemWithStuffInConstructor>();
			
			item.Should().BeNull();
		}
	}
}