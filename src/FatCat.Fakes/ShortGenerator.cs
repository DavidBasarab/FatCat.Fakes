using System;

namespace FatCat.Fakes
{
	internal class ShortGenerator : FakeGenerator
	{
		public override object Generate(Type type) => (short)Random.Next(short.MinValue, short.MaxValue);
	}
}