using System;
using System.Collections.Generic;

namespace FatCat.Fakes
{
	internal class FakeFactory
	{
		private static readonly Lazy<FakeFactory> instance = new Lazy<FakeFactory>(() => new FakeFactory());

		public static FakeFactory Instance => instance.Value;

		private Dictionary<Type, FakeGenerator> TypeGenerators { get; } = new Dictionary<Type, FakeGenerator>();
		
		public object GetValue(Type type)
		{
			var fakeType = GetTypeForFake(type);

			return TypeGenerators.TryGetValue(fakeType, out var faker) ? faker.Generate(type) : null;
		}
		
		private Type GetTypeForFake(Type type)
		{
			return type;

			// if (!type.IsGenericType) return type;
			//
			// var genericType = type.GetGenericTypeDefinition();
			//
			// return genericType == typeof(Nullable<>) ? type : genericType;
		}

		private FakeFactory()
		{
			TypeGenerators.Add(typeof(int), new IntGenerator());
			TypeGenerators.Add(typeof(int?), new IntGenerator());
		}
	}

	internal class IntGenerator : FakeGenerator
	{
		public override object Generate(Type type) => Random.Next();
	}

	internal abstract class FakeGenerator
	{
		private static Random random;

		protected static Random Random => random ??= new Random();

		public abstract object Generate(Type type);
	}

	public static class Faker
	{
		private static FakeFactory FakeFactory { get; } = FakeFactory.Instance;
		
		public static T Create<T>()
		{
			return (T)FakeFactory.GetValue(typeof(T));
		}

		public static object Create(Type fakeType) => FakeFactory.GetValue(fakeType);
	}
}