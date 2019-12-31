using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests
{
	public class ActionAfterItemCreate
	{
		[Fact]
		public void CanPassInMethodAfterCreateWithItem()
		{
			var item = Faker.Create<ItemToBeCreated>(i => { i.AnotherNumber = 5; });

			item.SomeNumber.Should().BeInRange(int.MinValue, int.MaxValue);
			item.AnotherNumber.Should().Be(5);
		}

		private class ItemToBeCreated
		{
			public int AnotherNumber { get; set; }

			public int SomeNumber { get; set; }

			public string SomeString { get; set; }
		}
	}
}