using System;
using System.Linq;

namespace FatCat.Fakes.Generators
{
	internal class StringGenerator : FakeGenerator
	{
		private const string StringCharacters = "TUV345ABCmnoSWXYpqrstuDEFZ01LMNOPQ26789abcdefghijklvwxyzGHIJKR";

		public override object Generate(Type typeToGenerate)
		{
			var length = Random.Next(8, 37);

			return Generate(length);
		}

		public string Generate(int length)
		{
			
			
			return new(Enumerable.Repeat(StringCharacters, length).Select(s => s[Random.Next() % s.Length]).ToArray());
		}
	}
}