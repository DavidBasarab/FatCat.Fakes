using System.Linq;

namespace FatCat.Fakes.Tests;

public class EnumGenerator
{
	public enum TestEnum
	{
		First,
		Second,
		Third,
		Fourth,
	}

	[Fact]
	public void WillSelectARandomEnum()
	{
		var selectedValues = Enumerable.Range(0, 100).Select(_ => Faker.Create<TestEnum>()).Distinct();

		selectedValues.Should().BeEquivalentTo(Enum.GetValues<TestEnum>());
	}
}
