using FluentAssertions;

namespace FatCat.Fakes.Tests.Primitives
{
	public class UshortFakeItem : FakeNumberItems<ushort>
	{
		protected override void VerifyRange(ushort value) => value.Should().BeInRange(ushort.MinValue, ushort.MaxValue);
	}
}