using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests
{
    public class ActionAfterItemCreate
    {
        [Fact]
        public void AfterCreateForNonGenericVersion()
        {
            var item =
                Faker.Create(
                    typeof(ItemToBeCreated),
                    i =>
                    {
                        var createdItem = i as ItemToBeCreated;

                        createdItem.AnotherNumber = 95;
                    }
                ) as ItemToBeCreated;

            item.SomeNumber.Should().BeInRange(int.MinValue, int.MaxValue);
            item.AnotherNumber.Should().Be(95);
        }

        [Fact]
        public void CanPassInMethodAfterCreateWithItem()
        {
            var item = Faker.Create<ItemToBeCreated>(i =>
            {
                i.AnotherNumber = 5;
            });

            item.SomeNumber.Should().BeInRange(int.MinValue, int.MaxValue);
            item.AnotherNumber.Should().Be(5);
        }

        private class ItemToBeCreated
        {
            public int AnotherNumber { get; set; }

            public int SomeNumber { get; }

            public string SomeString { get; set; }
        }
    }
}
