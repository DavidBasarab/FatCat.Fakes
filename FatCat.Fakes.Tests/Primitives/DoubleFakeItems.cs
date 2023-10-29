using FluentAssertions;

namespace FatCat.Fakes.Tests.Primitives
{
    public class DoubleFakeItems : FakeNumberItems<double>
    {
        protected override void VerifyRange(double value)
        {
            value.Should().BeInRange(double.MinValue, double.MaxValue);
        }
    }
}
