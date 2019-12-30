using FluentAssertions;

namespace FatCat.Fakes.Tests.Primitives
{
	public class ByteFakeItem : FakeNumberItems<byte>
	{
		protected override void VerifyRange(byte value) => value.Should().BeInRange(byte.MinValue, byte.MaxValue);
	}
}