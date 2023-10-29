using FluentAssertions;

namespace FatCat.Fakes.Tests.Primitives
{
    public class LongFakeItem : FakeNumberItems<long>
    {
        protected override void VerifyRange(long value) =>
            value.Should().BeInRange(long.MinValue, long.MaxValue);
    }
}
