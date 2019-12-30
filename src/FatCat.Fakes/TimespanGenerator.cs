using System;

namespace FatCat.Fakes
{
	internal class TimespanGenerator : FakeGenerator
	{
		public override object Generate(Type type) => new TimeSpan(Random.Next(0, 24), Random.Next(0, 59), Random.Next(0, 59));
	}
}