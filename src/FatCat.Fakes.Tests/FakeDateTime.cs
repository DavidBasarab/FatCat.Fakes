using System;
using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests
{
	public class FakeDateTime
	{
		[Fact]
		public void CanFakeADateTime()
		{
			var value = Faker.Create<DateTime>();

			value.Should().BeAfter(DateTime.MinValue);
			value.Should().BeBefore(DateTime.MaxValue);
		}
		
		[Fact]
		public void EachDateTimeWillBeRandom()
		{
			var previousValue = DateTime.MinValue;

			for (var i = 0; i < 13; i++)
			{
				var currentValue = Faker.Create<DateTime>();

				currentValue.Should().NotBe(previousValue);

				previousValue = currentValue;
			}
		}
	}
}