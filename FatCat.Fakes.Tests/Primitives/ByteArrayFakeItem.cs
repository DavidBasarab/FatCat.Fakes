using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests.Primitives
{
    public class ByteArrayFakeItem
    {
        [Fact]
        public void CanCreateAFakeByteArray()
        {
            var value = Faker.Create<byte[]>();

            value.Length.Should().BeGreaterThan(0);

            foreach (var byteValue in value)
            {
                byteValue.Should().BeInRange(byte.MinValue, byte.MaxValue);
            }
        }
    }
}
