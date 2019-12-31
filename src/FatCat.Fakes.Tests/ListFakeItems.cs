using System;
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
			listOfInts.Count.Should().BeLessOrEqualTo(9);

			foreach (var listItem in listOfInts) listItem.Should().BeInRange(int.MinValue, int.MaxValue);
		}
		
		[Fact]
		public void CanCreateAListOfDates()
		{
			var listOfDates = Faker.Create<List<DateTime>>();

			listOfDates.Count.Should().BeGreaterOrEqualTo(3);
			listOfDates.Count.Should().BeLessOrEqualTo(9);

			foreach (var dateItem in listOfDates)
			{
				dateItem.Should().BeAfter(DateTime.MinValue);
				dateItem.Should().BeBefore(DateTime.MaxValue);
			}
		}
		
		[Fact]
		public void CanCreateAListOfGivenLength()
		{
			var list = Faker.Create<List<string>>(17);

			list.Count.Should().Be(17);
		}
	}
}