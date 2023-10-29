using System;

namespace FatCat.Fakes.Generators
{
internal class ULongGenerator : FakeGenerator
{
	public override object Generate(Type typeToGenerate) => (ulong)Random.Next();
}
}