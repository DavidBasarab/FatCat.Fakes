using System;
using FatCat.Fakes.Generators;

namespace FatCat.Fakes
{
	public static class Faker
	{
		private static FakeFactory FakeFactory { get; } = FakeFactory.Instance;

		public static T Create<T>() => (T)FakeFactory.GetValue(typeof(T));

		public static object Create(Type fakeType) => FakeFactory.GetValue(fakeType);
	}
}