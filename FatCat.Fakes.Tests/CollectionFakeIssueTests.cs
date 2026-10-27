namespace FatCat.Fakes.Tests;

public class CollectionFakeIssueTests
{
	[Fact]
	public void CreateTestingMongoObject()
	{
		var item = Faker.Create<TestingMongoObject>();

		VerifyItem(item);
	}

	[Fact]
	public void CreateTheList()
	{
		var items = Faker.Create<List<TestingMongoObject>>(8);

		items.Count.Should().Be(8);

		foreach (var item in items)
		{
			VerifyItem(item);
		}
	}

	private static void VerifyItem(TestingMongoObject item)
	{
		item.Should().Not.BeNull();

		item.Name.Should().Not.BeNullOrEmpty();
		item.Number.Should().BeGreaterThan(0);
	}
}
