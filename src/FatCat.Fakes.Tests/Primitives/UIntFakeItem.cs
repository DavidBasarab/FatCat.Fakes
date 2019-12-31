using FluentAssertions;

namespace FatCat.Fakes.Tests.Primitives
{
	public class UIntFakeItem : FakeNumberItems<uint>
	{
		protected override void VerifyRange(uint value) => value.Should().BeInRange(uint.MinValue, uint.MaxValue);
	}
}