using System;

namespace FatCat.Fakes.Generators
{
	internal class UIntGenerator : FakeGenerator
	{
		public override object Generate(Type typeToGenerate) => (uint)Random.Next();
	}
}