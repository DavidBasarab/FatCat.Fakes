namespace FatCat.Fakes.Generators
{
	internal class ULongGenerator : FakeGenerator
	{
		public override object Generate() => (ulong)Random.Next();
	}
}