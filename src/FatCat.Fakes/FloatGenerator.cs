using System;

namespace FatCat.Fakes
{
	internal class FloatGenerator : FakeGenerator
	{
		public override object Generate(Type type) => (float)Random.NextDouble();
	}
}