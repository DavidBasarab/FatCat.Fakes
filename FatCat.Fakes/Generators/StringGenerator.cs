using System;
using System.Linq;

namespace FatCat.Fakes.Generators
{
	internal class StringGenerator : FakeGenerator
	{
		public override object Generate(Type typeToGenerate)
		{
			var length = Random.Next(8, 37);

			return Generate(length);
		}

		public string Generate(int length)
		{
			var characters = Guid.NewGuid().ToString();

			return new string(Enumerable.Repeat(characters, length).Select(s => s[Random.Next() % s.Length]).ToArray());
		}
	}
}