using System;

namespace FatCat.Fakes.Generators
{
	internal class UshortGenerator : FakeGenerator
	{
		public override object Generate(Type typeToGenerate) => (ushort)Random.Next(ushort.MinValue, ushort.MaxValue);
	}
}