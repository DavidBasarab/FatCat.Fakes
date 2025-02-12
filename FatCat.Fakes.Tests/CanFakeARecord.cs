namespace FatCat.Fakes.Tests;

public class CanFakeARecord
{
    [Fact]
    public void BasicTest()
    {
        var record = Faker.Create<RecordToFake>();

        VerifyRecord(record);
    }

    private static void VerifyRecord(RecordToFake record)
    {
        record.Should().NotBeNull();

        record.Number.Should().BeInRange(int.MinValue, int.MaxValue);
        record.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void CanFakeAListOfRecords()
    {
        var records = Faker.Create<List<RecordToFake>>(length: 4);

        records.Count.Should().Be(4);

        foreach (var record in records)
        {
            VerifyRecord(record);
        }
    }

    private record RecordToFake(int Number, string Name) { }
}
