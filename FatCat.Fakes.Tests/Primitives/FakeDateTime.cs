using System.Text.Json;

namespace FatCat.Fakes.Tests.Primitives;

public class FakeDateTime : PrimitiveTests<DateTime>
{
	[Fact]
	public void ANullableDateTimeCanBeFaked()
	{
		var value = Faker.Create<DateTime?>();

		value.Should().BeAfter(DateTime.MinValue);
		value.Should().BeBefore(DateTime.MaxValue);
	}

	[Fact]
	public void CanCreateAFakeDateTimeFromStaticMethod()
	{
		var value = Faker.RandomDateTime();

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
	public void CreateAUtcDateTime()
	{
		var value = Faker.Create<DateTime>();

		value.Kind.Should().Be(DateTimeKind.Utc);
	}

	[Fact]
	public void CreateAUtcNullableDateTime()
	{
		var value = Faker.Create<DateTime?>();

		value.Value.Kind.Should().Be(DateTimeKind.Utc);
	}

	[Fact]
	public void CreateAUtcRandomDateTime()
	{
		var value = Faker.RandomDateTime();

		value.Kind.Should().Be(DateTimeKind.Utc);
	}

	[Fact]
	public void SetAUtcDateTimeProperty()
	{
		var item = Faker.Create<ItemWithDates>();

		item.SomeDate.Kind.Should().Be(DateTimeKind.Utc);
	}

	[Fact]
	public void SetAUtcNullableDateTimeProperty()
	{
		var item = Faker.Create<ItemWithDates>();

		item.SomeNullableDate.Value.Kind.Should().Be(DateTimeKind.Utc);
	}

	[Fact]
	public void RoundTripThroughJsonUnchanged()
	{
		var dates = Faker.Create<List<DateTime>>(100_000);

		var readBack = JsonSerializer.Deserialize<List<DateTime>>(JsonSerializer.Serialize(dates));

		readBack.Should().Equal(dates);
	}

	private class ItemWithDates
	{
		public DateTime SomeDate { get; set; }

		public DateTime? SomeNullableDate { get; set; }
	}
}
