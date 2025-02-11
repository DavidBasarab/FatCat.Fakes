namespace FatCat.Fakes.Tests;

public class CanFakeARecord
{
    [Fact]
    public void BasicTest()
    {
        var record = Faker.Create<RecordToFake>();

        record.Number.Should().BeInRange(int.MinValue, int.MaxValue);
        record.Name.Should().NotBeNullOrEmpty();
    }

    private record RecordToFake(int Number, string Name) { }
}
