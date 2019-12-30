using System;

namespace FatCat.Fakes.Generators
{
	internal abstract class FakeGenerator
	{
		private static Random random;

		protected static Random Random => random ??= new Random();

		public abstract object Generate(Type type);
	}
}