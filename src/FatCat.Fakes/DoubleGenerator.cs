using System;

namespace FatCat.Fakes
{
	internal class DoubleGenerator : FakeGenerator
	{
		public override object Generate(Type type) => Random.NextDouble();
	}
}