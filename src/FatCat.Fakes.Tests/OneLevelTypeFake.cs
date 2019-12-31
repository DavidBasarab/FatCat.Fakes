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

			item.SomeList.Count.Should().BeInRange(3, 9);
		}

		private class ItemToFake
		{
			public byte SomeByte { get; set; }

			public DateTime SomeDateTime { get; set; }

			public double SomeDouble { get; set; }

			public long SomeLong { get; set; }

			public int SomeNumber { get; set; }

			public string SomeString { get; set; }

			public TimeSpan SomeTimeSpan { get; set; }

			public List<int> SomeList { get; set; }
		}
	}
}