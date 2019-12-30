using System;

namespace FatCat.Fakes.Generators
{
	internal class GuidGenerator : FakeGenerator
	{
		public override object Generate(Type type) => Guid.NewGuid();
	}
}