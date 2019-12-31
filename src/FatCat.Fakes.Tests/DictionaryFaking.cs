using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests
{
	public class DictionaryFaking
	{
		[Fact]
		public void WillRandomCreateKeysAndValuesForPrimitiveDictionary()
		{
			var item = Faker.Create<Dictionary<int, string>>();

			item.Count.Should().BeInRange(3, 9);

			foreach (var pair in item)
			{
				pair.Key.Should().BeInRange(int.MinValue, int.MaxValue);

				pair.Value.Length.Should().BeGreaterThan(5);
				pair.Value.Should().NotBeNullOrWhiteSpace();
			}
		}
		
		[Fact]
		public void CanCreateADictionaryWithTheNumberOfItemsIProvide()
		{
			var item = Faker.Create<Dictionary<int, string>>(length: 21);

			item.Count.Should().Be(21);
		}
	}
}