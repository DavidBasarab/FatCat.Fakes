using System;
using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests
{
	public class IgnoreProperties
	{
		[Fact]
		public void CanIgnoreANonNullableType()
		{
			var item = Faker.Create<TestFakingItem>(propertiesToIgnore: i => i.SomeNumber);

			item.SomeString.Should().NotBeNullOrWhiteSpace();
			item.SomeNumber.Should().Be(default);

			item.SubObject.Should().NotBeNull();

			item.SubObject.First.Should().BeInRange(int.MinValue, int.MaxValue);
			item.SubObject.Second.Should().BeInRange(int.MinValue, int.MaxValue);
			item.SubObject.Date.Should().BeAfter(DateTime.MinValue);
		}

		[Fact]
		public void WillNotSetTheIgnoredProperty()
		{
			var item = Faker.Create<TestFakingItem>(propertiesToIgnore: i => i.SomeString);

			item.SomeString.Should().BeNullOrWhiteSpace();
			item.SomeNumber.Should().BeInRange(int.MinValue, int.MaxValue);

			item.SubObject.Should().NotBeNull();

			item.SubObject.First.Should().BeInRange(int.MinValue, int.MaxValue);
			item.SubObject.Second.Should().BeInRange(int.MinValue, int.MaxValue);
			item.SubObject.Date.Should().BeAfter(DateTime.MinValue);
		}

		public class SubObject
		{
			public DateTime Date { get; set; }

			public int First { get; set; }

			public int Second { get; set; }
		}

		public class TestFakingItem
		{
			public int SomeNumber { get; set; }

			public string SomeString { get; set; }

			public SubObject SubObject { get; set; }
		}
	}
}