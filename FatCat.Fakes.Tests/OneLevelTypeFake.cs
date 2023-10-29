using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests
{
    public class OneLevelTypeFake
    {
        [Fact]
        public void CanFakeABasicItemWithJustPrimitiveTypes()
        {
            var item = Faker.Create<ItemToFake>();

            item.Should().NotBeNull();

            item.SomeByte.Should().BeInRange(byte.MinValue, byte.MaxValue);
            item.SomeString.Length.Should().BeGreaterThan(7);
            item.SomeNumber.Should().BeInRange(int.MinValue, int.MaxValue);
            item.SomeDouble.Should().BeInRange(double.MinValue, double.MaxValue);
            item.SomeLong.Should().BeInRange(long.MinValue, long.MaxValue);

            item.SomeDateTime.Should().BeAfter(DateTime.MinValue);
            item.SomeDateTime.Should().BeBefore(DateTime.MaxValue);

            item.SomeTimeSpan.Should().BeGreaterThan(TimeSpan.MinValue);
            item.SomeTimeSpan.Should().BeLessThan(TimeSpan.MaxValue);

            item.SomeList.Count.Should().BeInRange(1, 3);

            item.PrivateSetInt.Should().BeInRange(int.MinValue, int.MaxValue);

            item.NoSetter.Should().Be(12);
        }

        [Fact]
        public void WillNotSetAPrivateField()
        {
            var item = Faker.Create<ItemToFake>();

            item.GetPrivateItemValue().Should().Be(34);
        }

        private class ItemToFake
        {
            private readonly int aPrivateItem = 34;

            public int NoSetter { get; } = 12;

            public int PrivateSetInt { get; }

            public byte SomeByte { get; }

            public DateTime SomeDateTime { get; }

            public double SomeDouble { get; }

            public List<int> SomeList { get; }

            public long SomeLong { get; }

            public int SomeNumber { get; }

            public string SomeString { get; }

            public TimeSpan SomeTimeSpan { get; }

            public int GetPrivateItemValue()
            {
                return aPrivateItem;
            }
        }
    }
}
