namespace FatCat.Fakes.Generators
{
	internal class UshortGenerator : FakeGenerator
	{
		public override object Generate() => (ushort)Random.Next(ushort.MinValue, ushort.MaxValue);
	}
}