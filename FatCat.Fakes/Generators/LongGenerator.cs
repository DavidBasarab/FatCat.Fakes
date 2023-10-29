using System;

namespace FatCat.Fakes.Generators
{
internal class LongGenerator : FakeGenerator
{
	public override object Generate(Type typeToGenerate) => (long)Random.Next();
}
}