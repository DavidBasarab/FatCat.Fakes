using System;

namespace FatCat.Fakes.Generators
{
internal class GuidGenerator : FakeGenerator
{
	public override object Generate(Type typeToGenerate) => Guid.NewGuid();
}
}