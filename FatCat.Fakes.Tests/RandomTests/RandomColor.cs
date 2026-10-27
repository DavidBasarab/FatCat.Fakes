using System.Drawing;

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

                ((object)currentColor).Should().Not.Be(previousColor);

                previousColor = currentColor;
            }
        }
    }
}
