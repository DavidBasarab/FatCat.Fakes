using System;

namespace FatCat.Fakes.Generators
{
internal class DoubleGenerator : FakeGenerator
{
	public override object Generate(Type typeToGenerate) => Random.NextDouble();
}
}