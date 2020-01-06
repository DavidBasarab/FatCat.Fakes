using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests.RandomTests
{
	public class RandomString
	{
		[Fact]
		public void CanCreateARandomString()
		{
			var randomString = Faker.RandomString();

			randomString.Length.Should().BeGreaterThan(7);
			randomString.Should().NotBeNullOrWhiteSpace();
		}
	}
}