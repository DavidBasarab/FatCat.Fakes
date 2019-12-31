using System;
using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests
{
	public class WithONeAbstractClass
	{
		[Fact]
		public void WillPopulateAnAbstractClass()
		{
			var item = Faker.Create<SomeClass>();

			item.ThisIsNotReal.Should().NotBeNull();
			item.ThisIsNotReal.Should().BeOfType<RealClass>();

			item.ThisIsNotReal.SomeInt.Should().BeInRange(int.MinValue, int.MaxValue);
		}

		private abstract class AbstractClass
		{
			public int SomeInt { get; set; }

			public string SomeString { get; set; }
		}

		private class RealClass : AbstractClass
		{
			public DateTime SomeDateTime { get; set; }
		}

		private class SomeClass
		{
			public AbstractClass ThisIsNotReal { get; set; }
		}
	}
}