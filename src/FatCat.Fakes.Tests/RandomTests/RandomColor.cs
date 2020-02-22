using System.Drawing;
using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests.RandomTests
{
	public class RandomColor
	{
		[Fact]
		public void CanGenerateARandomColor()
		{
			Color previousColor = default;

			for (var i = 0; i < 7; i++)
			{
				var currentColor = Faker.RandomColor();

				currentColor.Should().NotBe(previousColor);

				previousColor = currentColor;
			}
		}
	}
}