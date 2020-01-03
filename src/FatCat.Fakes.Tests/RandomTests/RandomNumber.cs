using System;
using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests.RandomTests
{
	public class RandomNumber
	{
		[Fact]
		public void CanGetARandomInt()
		{
			var value = Faker.RandomInt();

			value.Should().BeInRange(int.MinValue, int.MaxValue);
		}
	}
}