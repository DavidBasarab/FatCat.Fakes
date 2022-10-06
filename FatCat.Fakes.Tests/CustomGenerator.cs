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
		
		[Fact]
		public void AddingMultipleGeneratorsWillNotError()
		{
			var typeToFake = typeof(ItemForGenerator);

			Faker.AddGenerator(typeToFake, new TestingGenerator());
			Faker.AddGenerator(typeToFake, new TestingGenerator());
			Faker.AddGenerator(typeToFake, new TestingGenerator());
		}

		[Fact]
		public void WillUseTheNewGenerator()
		{
			var typeToFake = typeof(ItemForGenerator);

			Faker.AddGenerator(typeToFake, new TestingGenerator());

			var item = Faker.Create<ItemForGenerator>();

			item.SomeDate.Should().Be(new DateTime(1969, 07, 20));
			item.SomeNumber.Should().Be(11);
			item.SomeString.Should().Be("Moon");
		}

		public class ItemForGenerator
		{
			public DateTime SomeDate { get; set; }

			public int SomeNumber { get; set; }

			public string SomeString { get; set; }
		}

		public class TestingGenerator : FakeGenerator
		{
			public override object Generate(Type typeToGenerate) => new ItemForGenerator
																	{
																		SomeDate = new DateTime(1969, 07, 20),
																		SomeNumber = 11,
																		SomeString = "Moon"
																	};
		}
	}
}