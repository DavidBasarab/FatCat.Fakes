using System.Linq;

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

public class IdObjectLevel
{
    public DummyMongoObjectId ObjectId { get; set; }
}

public class TestingMongoObject : IdObjectLevel
{
    public string Name { get; set; }

    public int Number { get; set; }

    public DateTime SomeDate { get; set; }
}

public struct DummyMongoObjectId
{
    // Constructor to create ObjectId from byte array
    public DummyMongoObjectId(byte[] id) => Id = id;

    // Property to get the ObjectId as a byte array
    public byte[] Id { get; }

    // Override ToString to return the ObjectId as a hex string
    public override string ToString()
    {
        return string.Join("", Id.Select(b => b.ToString("x2")));
    }

    // Equality comparison
    public override bool Equals(object obj)
    {
        if (obj is DummyMongoObjectId other)
        {
            return Id.SequenceEqual(other.Id);
        }

        return false;
    }

    public override int GetHashCode() => BitConverter.ToInt32(Id, 0);

    // Static method to generate a new ObjectId (random)
    public static DummyMongoObjectId NewObjectId()
    {
        var randomBytes = new byte[12];
        var rng = new Random();
        rng.NextBytes(randomBytes);

        return new DummyMongoObjectId(randomBytes);
    }
}
