using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests
{
    public class CanFakeAnInterface
    {
        private interface IAmAnInterface
        {
            int SomeNumber { get; set; }

            string SomeString { get; set; }
        }

        public class FirstImplementation : IAmAnInterface
        {
            public int SomeNumber { get; set; }

            public string SomeString { get; set; }
        }

        [Fact]
        public void CanCreateAnInterface()
        {
            var item = Faker.Create<IAmAnInterface>();

            item.Should().BeOfType<FirstImplementation>();

            item.SomeNumber.Should().BeInRange(int.MinValue, int.MaxValue);

            item.SomeString.Should().NotBeNullOrWhiteSpace();
        }
    }
}
