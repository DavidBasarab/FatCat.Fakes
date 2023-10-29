using FluentAssertions;

namespace FatCat.Fakes.Tests.Primitives
{
    public class IntFakeItems : FakeNumberItems<int>
    {
        protected override void VerifyRange(int value) =>
            value.Should().BeInRange(int.MinValue, int.MaxValue);
    }
}
