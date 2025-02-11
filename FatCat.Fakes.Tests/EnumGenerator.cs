namespace FatCat.Fakes.Tests;

public class EnumGenerator
{
    public enum TestEnum
    {
        First,
        Second,
        Third,
        Fourth
    }

    [Fact]
    public void WillSelectARandomEnum()
    {
        var firstFound = false;
        var secondFound = false;
        var thirdFound = false;
        var fourthFound = false;

        for (var i = 0; i < 16; i++)
        {
            var item = Faker.Create<TestEnum>();

            firstFound = firstFound || item == TestEnum.First;
            secondFound = secondFound || item == TestEnum.Second;
            thirdFound = thirdFound || item == TestEnum.Third;
            fourthFound = fourthFound || item == TestEnum.Fourth;
        }

        firstFound.Should().BeTrue();
        secondFound.Should().BeTrue();
        thirdFound.Should().BeTrue();
        firstFound.Should().BeTrue();
    }
}
