using System;

namespace FatCat.Fakes.Generators
{
	internal class FloatGenerator : FakeGenerator
	{
		public override object Generate(Type type) => (float)Random.NextDouble();
	}
}