using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests
{
    public class ObjectWithListOfSubObjects
    {
        [Fact]
        public void ItWillPopulateALowerObject()
        {
            var upperObject = Faker.Create<UpperObject>();

            upperObject.LowerList.Should().NotBeNull();
            upperObject.LowerList.Count.Should().BeInRange(1, 4);

            upperObject.SomeNumber.Should().BeInRange(int.MinValue, int.MaxValue);
            upperObject.SomeString.Length.Should().BeGreaterThan(6);

            foreach (var lowerObject in upperObject.LowerList)
            {
                lowerObject.SomeNumber.Should().BeInRange(int.MinValue, int.MaxValue);
                lowerObject.SomeString.Length.Should().BeGreaterThan(6);
            }
        }

        private class LowerObject
        {
            public int SomeNumber { get; }

            public string SomeString { get; }
        }

        private class UpperObject
        {
            public List<LowerObject> LowerList { get; }

            public int SomeNumber { get; }

            public string SomeString { get; }
        }
    }
}
