using System.Linq;

namespace FatCat.Fakes.Tests;

public class ListFakeItems
{
    [Fact]
    public void ArrayShouldNotBeGreaterThan3()
    {
        for (var i = 0; i < 10; i++)
        {
            var bytes = Faker.Create<byte[]>();

            bytes.Length.Should().BeLessThan(4);
        }
    }

    [Fact]
    public void CanCreateAListOfAnyPrimitive()
    {
        var listOfInts = Faker.Create<List<int>>();

        listOfInts.Count.Should().BeGreaterOrEqualTo(1);
        listOfInts.Count.Should().BeLessOrEqualTo(3);

        foreach (var listItem in listOfInts)
        {
            listItem.Should().BeInRange(int.MinValue, int.MaxValue);
        }
    }

    [Fact]
    public void CanCreateAListOfDates()
    {
        var listOfDates = Faker.Create<List<DateTime>>();

        listOfDates.Count.Should().BeGreaterOrEqualTo(1);
        listOfDates.Count.Should().BeLessOrEqualTo(3);

        foreach (var dateItem in listOfDates)
        {
            dateItem.Should().BeAfter(DateTime.MinValue);
            dateItem.Should().BeBefore(DateTime.MaxValue);
        }
    }

    [Fact]
    public void CanCreateAListOfGivenLength()
    {
        var list = Faker.Create<List<string>>(17);

        list.Count.Should().Be(17);
    }

    [Fact]
    public void CanCreateAnArray()
    {
        var array = Faker.Create<short[]>(7);

        var testList = array.ToList();

        VerifyItems(testList);
    }

    [Fact]
    public void CanCreateAnIEnumerable()
    {
        var enumerableOfItems = Faker.Create<IEnumerable<short>>(7);

        var testList = enumerableOfItems.ToList();

        VerifyItems(testList);
    }

    private static void VerifyItems(IReadOnlyCollection<short> testList)
    {
        testList.Count.Should().Be(7);

        foreach (var testItem in testList)
        {
            testItem.Should().BeInRange(short.MinValue, short.MaxValue);
        }
    }
}
