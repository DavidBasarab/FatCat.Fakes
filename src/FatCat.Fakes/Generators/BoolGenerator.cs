namespace FatCat.Fakes.Generators
{
	internal class BoolGenerator : FakeGenerator
	{
		public override object Generate() => Random.Next() % 2 == 0;
	}
}