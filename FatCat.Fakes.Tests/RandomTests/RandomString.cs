namespace FatCat.Fakes.Tests.RandomTests;

public class RandomString
{
	[Fact]
	public void CanCreateARandomString()
	{
		var randomString = Faker.RandomString();

		randomString.Length.Should().BeGreaterThan(7);
		randomString.Should().Not.BeNullOrWhiteSpace();
	}

	[Fact]
	public void CanCreateARandomStringOfGivenLength()
	{
		var randomString = Faker.RandomString(length: 17);

		randomString.Length.Should().Be(17);
		randomString.Should().Not.BeNullOrWhiteSpace();
	}

	[Fact]
	public void CanStartARandomStringWithProvidedValue()
	{
		var stringWithPrefix = Faker.RandomString("ShouldStartWithThis");

		stringWithPrefix.Should().StartWith("ShouldStartWithThis");
	}

	[Fact]
	public void CreateAStringThatIsNotAllTheSameCharacter()
	{
		Faker.RandomString().Should().Not.Be("AAAAAAAA");
	}
}
