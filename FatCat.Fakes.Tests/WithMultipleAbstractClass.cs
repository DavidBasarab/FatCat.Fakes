namespace FatCat.Fakes.Tests;

public class WithMultipleAbstractClass
{
    [Fact]
    public void CanFakeAnAbstractClass()
    {
        var item = Faker.Create<AbstractClass>();

        item.Should().NotBeNull();

        item.SomeInt.Should().BeInRange(int.MinValue, int.MaxValue);
    }

    [Fact]
    public void WillPickTypeAtRandom()
    {
        var realClassFound = false;
        var anotherClassFound = false;
        var evenAnotherClassFound = false;

        for (var i = 0; i < 35; i++)
        {
            var item = Faker.Create<SomeClass>();

            if (item.ThisIsNotReal is RealClass)
            {
                realClassFound = true;
            }

            if (item.ThisIsNotReal is AnotherClass)
            {
                anotherClassFound = true;
            }

            if (item.ThisIsNotReal is EvenAnotherClass)
            {
                evenAnotherClassFound = true;
            }
        }

        realClassFound.Should().BeTrue();
        anotherClassFound.Should().BeTrue();
        evenAnotherClassFound.Should().BeTrue();
    }

    private abstract class AbstractClass
    {
        public int SomeInt { get; set; }

        public string SomeString { get; set; }
    }

    private class AnotherClass : AbstractClass
    {
        public int ANewNumber { get; set; }
    }

    private class EvenAnotherClass : AbstractClass
    {
        public string SomeName { get; set; }
    }

    private class RealClass : AbstractClass
    {
        public DateTime SomeDateTime { get; set; }
    }

    private class SomeClass
    {
        public AbstractClass ThisIsNotReal { get; set; }
    }
}
