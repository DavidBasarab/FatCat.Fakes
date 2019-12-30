using System;
using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests
{
	public abstract class PrimitiveTests<T>
	{
		[Fact]
		public void EachItemWillBeRandom()
		{
			var previousValue = default(T);

			for (var i = 0; i < 13; i++)
			{
				var currentValue = Faker.Create<T>();

				currentValue.Should().NotBe(previousValue);

				previousValue = currentValue;
			}
		}
	}

	public class TimeSpanFakeDateTime : PrimitiveTests<TimeSpan>
	{
		[Fact]
		public void CanFakeATimeSpan()
		{
			var value = Faker.Create<TimeSpan>();

			value.Should().BeGreaterThan(TimeSpan.MinValue);
			value.Should().BeLessThan(TimeSpan.MaxValue);
		}
	}

	public class FakeDateTime
	{
		[Fact]
		public void ANullableDateTimeCanBeFaked()
		{
			var value = Faker.Create<DateTime?>();

			value.Should().BeAfter(DateTime.MinValue);
			value.Should().BeBefore(DateTime.MaxValue);
		}

		[Fact]
		public void CanFakeADateTime()
		{
			var value = Faker.Create<DateTime>();

			value.Should().BeAfter(DateTime.MinValue);
			value.Should().BeBefore(DateTime.MaxValue);
		}

		[Fact]
		public void EachDateTimeWillBeRandom()
		{
			var previousValue = DateTime.MinValue;

			for (var i = 0; i < 13; i++)
			{
				var currentValue = Faker.Create<DateTime>();

				currentValue.Should().NotBe(previousValue);

				previousValue = currentValue;
			}
		}
	}
}