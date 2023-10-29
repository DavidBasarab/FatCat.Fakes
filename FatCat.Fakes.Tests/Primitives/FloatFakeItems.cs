using FluentAssertions;

namespace FatCat.Fakes.Tests.Primitives
{
    public class FloatFakeItems : FakeNumberItems<float>
    {
        protected override void VerifyRange(float value)
        {
            value.Should().BeInRange(ushort.MinValue, ushort.MaxValue);
        }
    }
}
