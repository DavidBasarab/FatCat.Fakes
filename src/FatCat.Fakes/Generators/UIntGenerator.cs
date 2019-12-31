namespace FatCat.Fakes.Generators
{
	internal class UIntGenerator : FakeGenerator
	{
		public override object Generate() => (uint)Random.Next();
	}
}