using FluentAssertions;

namespace FatCat.Fakes.Tests.Primitives
{
    public class ShortFakeItems : FakeNumberItems<short>
    {
        protected override void VerifyRange(short value) =>
            value.Should().BeInRange(short.MinValue, short.MaxValue);
    }
}
