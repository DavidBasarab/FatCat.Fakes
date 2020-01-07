using System;
using FatCat.Fakes.Generators;
using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests
{
	public class CustomGenerator
	{
		[Fact]
		public void CanDefineACustomGenerator()
		{
			var typeToFake = typeof(ItemForGenerator);

			Faker.AddGenerator(typeToFake, new TestingGenerator());

			Faker.FakeFactory.IsTypeFaked(typeToFake).Should().BeTrue();
		}

		public class ItemForGenerator { }

		public class TestingGenerator : FakeGenerator
		{
			public override object Generate() => throw new NotImplementedException();
		}
	}
}