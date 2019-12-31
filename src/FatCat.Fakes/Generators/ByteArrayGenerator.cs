namespace FatCat.Fakes.Generators
{
	internal class ByteArrayGenerator : FakeGenerator
	{
		public override object Generate()
		{
			var length = Random.Next(12, 39);

			var array = new byte[length];

			var byteGenerator = new ByteGenerator();

			for (var i = 0; i < length; i++) array[i] = (byte)byteGenerator.Generate();

			return array;
		}
	}
}