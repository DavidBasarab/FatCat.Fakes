using System;

namespace FatCat.Fakes.Generators
{
	internal class ByteGenerator : FakeGenerator
	{
		public override object Generate(Type type) => (byte)Random.Next(byte.MinValue, byte.MaxValue);
	}
}