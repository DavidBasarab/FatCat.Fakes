namespace FatCat.Fakes.Generators
{
	internal class FloatGenerator : FakeGenerator
	{
		public override object Generate() => (float)Random.NextDouble();
	}
}