using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests
{
	public class ListFakeItems
	{
		[Fact]
		public void CanCreateAListOfAnyPrimitive()
		{
			var listOfInts = Faker.Create<List<int>>();

			listOfInts.Count.Should().BeGreaterOrEqualTo(3);

			foreach (var listItem in listOfInts) listItem.Should().BeInRange(int.MinValue, int.MaxValue);
		}
	}
}