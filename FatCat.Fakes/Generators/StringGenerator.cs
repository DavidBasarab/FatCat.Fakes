using System;
using System.Linq;

namespace FatCat.Fakes.Generators
{
	internal class StringGenerator : FakeGenerator
	{
		private const string StringCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";

		public override object Generate(Type typeToGenerate)
		{
			var length = Random.Next(8, 37);

			return Generate(length);
		}

		public string Generate(int length) => new string(Enumerable.Repeat(StringCharacters, length).Select(s => s[Random.Next(s.Length)]).ToArray());
	}
}