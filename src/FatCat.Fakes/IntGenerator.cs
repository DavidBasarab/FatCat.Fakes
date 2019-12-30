using System;

namespace FatCat.Fakes
{
	internal class IntGenerator : FakeGenerator
	{
		public override object Generate(Type type) => Random.Next();
	}
}