using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests
{
	public class Class1
	{
		[Fact]
		public void Test_Name() { true.Should().BeTrue(); }
	}
}