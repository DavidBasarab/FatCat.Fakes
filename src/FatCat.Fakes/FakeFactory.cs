using System;
using System.Collections.Generic;

namespace FatCat.Fakes
{
	internal class FakeFactory
	{
		private static readonly Lazy<FakeFactory> instance = new Lazy<FakeFactory>(() => new FakeFactory());

		public static FakeFactory Instance => instance.Value;

		private Dictionary<Type, FakeGenerator> TypeGenerators { get; } = new Dictionary<Type, FakeGenerator>();

		private FakeFactory()
		{
			TypeGenerators.Add(typeof(int), new IntGenerator());
			TypeGenerators.Add(typeof(int?), new IntGenerator());
			TypeGenerators.Add(typeof(DateTime), new DateTimeGenerator());
			TypeGenerators.Add(typeof(DateTime?), new DateTimeGenerator());
			TypeGenerators.Add(typeof(TimeSpan), new TimespanGenerator());
			TypeGenerators.Add(typeof(TimeSpan?), new TimespanGenerator());
		}

		public object GetValue(Type type)
		{
			var fakeType = GetTypeForFake(type);

			return TypeGenerators.TryGetValue(fakeType, out var faker) ? faker.Generate(type) : null;
		}

		private Type GetTypeForFake(Type type) => type;

		// if (!type.IsGenericType) return type;
		//
		// var genericType = type.GetGenericTypeDefinition();
		//
		// return genericType == typeof(Nullable<>) ? type : genericType;
	}
}