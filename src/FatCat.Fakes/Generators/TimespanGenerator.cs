using System;

namespace FatCat.Fakes.Generators
{
	internal class TimespanGenerator : FakeGenerator
	{
		public override object Generate() => new TimeSpan(Random.Next(0, 24), Random.Next(0, 59), Random.Next(0, 59));
	}
}