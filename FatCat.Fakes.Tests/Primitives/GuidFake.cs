using System;
using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests.Primitives
{
public abstract class GuidFake : PrimitiveTests<Guid>
{
	[Fact]
	public void CanFakeAGuid()
	{
		var value = Faker.Create<Guid>();

		value.Should().NotBeEmpty();
	}

	[Fact]
	public void CanFakeANullableGuid()
	{
		var value = Faker.Create<Guid?>();

		value.Should().NotBeEmpty();
	}
}
}