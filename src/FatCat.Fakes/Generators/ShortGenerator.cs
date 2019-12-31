namespace FatCat.Fakes.Generators
{
	internal class ShortGenerator : FakeGenerator
	{
		public override object Generate() => (short)Random.Next(short.MinValue, short.MaxValue);
	}
}