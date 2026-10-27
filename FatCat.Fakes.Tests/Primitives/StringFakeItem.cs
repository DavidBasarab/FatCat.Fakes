namespace FatCat.Fakes.Tests.Primitives;

public class StringFakeItem : PrimitiveTests<string>
{
    [Fact]
    public void CanFakeAString()
    {
        var value = Faker.Create<string>();

        value.Should().Not.BeEmpty();
        value.Length.Should().BeGreaterThan(7);
    }
}
