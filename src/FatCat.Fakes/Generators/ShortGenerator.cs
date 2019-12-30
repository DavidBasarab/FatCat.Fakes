using System;

namespace FatCat.Fakes.Generators
{
	internal class ShortGenerator : FakeGenerator
	{
		public override object Generate(Type type) => (short)Random.Next(short.MinValue, short.MaxValue);
	}
}