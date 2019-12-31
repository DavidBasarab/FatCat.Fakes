namespace FatCat.Fakes.Generators
{
	internal class LongGenerator : FakeGenerator
	{
		public override object Generate() => (long)Random.Next();
	}
}