using FluentAssertions;

namespace FatCat.Fakes.Tests.Primitives
{
	public class UlongFakeItem : FakeNumberItems<ulong>
	{
		protected override void VerifyRange(ulong value) => value.Should().BeInRange(ulong.MinValue, ulong.MaxValue);
	}
}