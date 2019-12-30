using System;

namespace FatCat.Fakes
{
	internal class UshortGenerator : FakeGenerator
	{
		public override object Generate(Type type) => (ushort)Random.Next(ushort.MinValue, ushort.MaxValue);
	}
}