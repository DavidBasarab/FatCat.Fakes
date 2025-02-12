using MongoDB.Bson;

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
        item.Should().NotBeNull();

        item.Name.Should().NotBeNullOrEmpty();
        item.Number.Should().BeGreaterThan(0);
    }
}

public class BottomLevel { }

public class IdObjectLevel : BottomLevel
{
    public ObjectId Id { get; set; }
}

public class TestingMongoObject : IdObjectLevel
{
    public string Name { get; set; }

    public int Number { get; set; }

    public DateTime SomeDate { get; set; }
}
