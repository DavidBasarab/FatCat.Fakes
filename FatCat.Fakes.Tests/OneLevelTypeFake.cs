namespace FatCat.Fakes.Tests;

public class OneLevelTypeFake
{
    [Fact]
    public void CanFakeABasicItemWithJustPrimitiveTypes()
    {
        var item = Faker.Create<ItemToFake>();

        item.Should().NotBeNull();

        item.SomeByte.Should().BeInRange(byte.MinValue, byte.MaxValue);
        item.SomeString.Length.Should().BeGreaterThan(7);
        item.SomeNumber.Should().BeInRange(int.MinValue, int.MaxValue);
        item.SomeDouble.Should().BeInRange(double.MinValue, double.MaxValue);
        item.SomeLong.Should().BeInRange(long.MinValue, long.MaxValue);

        item.SomeDateTime.Should().BeAfter(DateTime.MinValue);
        item.SomeDateTime.Should().BeBefore(DateTime.MaxValue);

        item.SomeTimeSpan.Should().BeGreaterThan(TimeSpan.MinValue);
        item.SomeTimeSpan.Should().BeLessThan(TimeSpan.MaxValue);

        item.SomeList.Count.Should().BeInRange(1, 3);

        item.PrivateSetInt.Should().BeInRange(int.MinValue, int.MaxValue);

        item.NoSetter.Should().Be(12);
    }

    [Fact]
    public void WillNotSetAPrivateField()
    {
        var item = Faker.Create<ItemToFake>();

        item.GetPrivateItemValue().Should().Be(34);
    }

    private class ItemToFake
    {
        private readonly int aPrivateItem = 34;

        public int NoSetter { get; } = 12;

        public int PrivateSetInt { get; set; }

        public byte SomeByte { get; set; }

        public DateTime SomeDateTime { get; set; }

        public double SomeDouble { get; set; }

        public List<int> SomeList { get; set; }

        public long SomeLong { get; set; }

        public int SomeNumber { get; set; }

        public string SomeString { get; set; }

        public TimeSpan SomeTimeSpan { get; set; }

        public int GetPrivateItemValue()
        {
            return aPrivateItem;
        }
    }
}
