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

	[Fact]
	public void CanCreateARandomStringOfGivenLength()
	{
		var randomString = Faker.RandomString(length: 17);

		randomString.Length.Should().Be(17);
		randomString.Should().NotBeNullOrWhiteSpace();
	}

	[Fact]
	public void CanStartARandomStringWithProvidedValue()
	{
		var stringWithPrefix = Faker.RandomString("ShouldStartWithThis");

		stringWithPrefix.Should().StartWith("ShouldStartWithThis");
	}

	[Fact]
	public void TwoRandomStringsShouldNotBeTheSame() => Faker.RandomString().Should().NotBe("AAAAAAAA");
}
}