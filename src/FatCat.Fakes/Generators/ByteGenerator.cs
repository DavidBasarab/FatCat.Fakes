namespace FatCat.Fakes.Generators
{
	internal class ByteGenerator : FakeGenerator
	{
		public override object Generate() => (byte)Random.Next(byte.MinValue, byte.MaxValue);
	}
}