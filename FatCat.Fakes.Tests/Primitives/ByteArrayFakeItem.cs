using System.Linq;
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

        [Fact]
        public void StaticRandomByteArrayMethod()
        {
            var length = 1034;

            var bytes = Faker.RandomBytes(length);

            bytes.Length

            bytes.LongCount().Should().BeGreaterThan(length);

            foreach (var byteValue in bytes)
            {
                byteValue.Should().BeInRange(byte.MinValue, byte.MaxValue);
            }
        }
    }
}
