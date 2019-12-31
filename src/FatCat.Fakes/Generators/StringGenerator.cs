using System;
using System.Linq;

namespace FatCat.Fakes.Generators
{
	internal class StringGenerator : FakeGenerator
	{
		private const string StringCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";

		public override object Generate(Type type)
		{
			var length = Random.Next(8, 37);

			return new string(Enumerable.Repeat(StringCharacters, length).Select(s => s[Random.Next(s.Length)]).ToArray());
		}
	}
}