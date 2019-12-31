namespace FatCat.Fakes.Generators
{
	internal class DoubleGenerator : FakeGenerator
	{
		public override object Generate() => Random.NextDouble();
	}
}