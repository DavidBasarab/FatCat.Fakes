namespace FatCat.Fakes.Generators
{
	internal class IntGenerator : FakeGenerator
	{
		public override object Generate() => Random.Next();
	}
}