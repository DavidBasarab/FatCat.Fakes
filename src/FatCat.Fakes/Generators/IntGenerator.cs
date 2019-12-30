using System;

namespace FatCat.Fakes.Generators
{
	internal class IntGenerator : FakeGenerator
	{
		public override object Generate(Type type) => Random.Next();
	}
}