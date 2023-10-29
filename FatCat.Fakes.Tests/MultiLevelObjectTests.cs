using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests
{
    public class MultiLevelObjectTests
    {
        [Fact]
        public void ItWillPopulateALowerObject()
        {
            var upperObject = Faker.Create<UpperObject>();

            upperObject.LowerObject.Should().NotBeNull();

            upperObject.SomeNumber.Should().BeInRange(int.MinValue, int.MaxValue);
            upperObject.SomeString.Length.Should().BeGreaterThan(6);

            upperObject.LowerObject.SomeNumber.Should().BeInRange(int.MinValue, int.MaxValue);
            upperObject.LowerObject.SomeString.Length.Should().BeGreaterThan(6);
        }

        private class LowerObject
        {
            public int SomeNumber { get; }

            public string SomeString { get; }
        }

        private class UpperObject
        {
            public LowerObject LowerObject { get; }

            public int SomeNumber { get; }

            public string SomeString { get; }
        }
    }
}
