using System;

namespace FatCat.Fakes
{
	internal class GuidGenerator : FakeGenerator
	{
		public override object Generate(Type type) => Guid.NewGuid();
	}
}