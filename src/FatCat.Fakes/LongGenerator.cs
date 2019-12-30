using System;

namespace FatCat.Fakes
{
	internal class LongGenerator : FakeGenerator
	{
		public override object Generate(Type type) => (long)Random.Next();
	}
}