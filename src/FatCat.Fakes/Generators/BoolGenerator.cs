using System;

namespace FatCat.Fakes.Generators
{
	internal class BoolGenerator : FakeGenerator
	{
		public override object Generate(Type type) => Random.Next() % 2 == 0;
	}
}