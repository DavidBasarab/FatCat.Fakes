using System;
using System.Collections.Generic;

namespace FatCat.Fakes.Generators
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
			TypeGenerators.Add(typeof(double), new DoubleGenerator());
			TypeGenerators.Add(typeof(double?), new DoubleGenerator());
			TypeGenerators.Add(typeof(byte), new ByteGenerator());
			TypeGenerators.Add(typeof(byte?), new ByteGenerator());
			TypeGenerators.Add(typeof(short), new ShortGenerator());
			TypeGenerators.Add(typeof(short?), new ShortGenerator());
			TypeGenerators.Add(typeof(ushort), new UshortGenerator());
			TypeGenerators.Add(typeof(ushort?), new UshortGenerator());
			TypeGenerators.Add(typeof(float), new FloatGenerator());
			TypeGenerators.Add(typeof(float?), new FloatGenerator());
			TypeGenerators.Add(typeof(long), new LongGenerator());
			TypeGenerators.Add(typeof(long?), new LongGenerator());
			TypeGenerators.Add(typeof(Guid), new GuidGenerator());
			TypeGenerators.Add(typeof(Guid?), new GuidGenerator());
			TypeGenerators.Add(typeof(bool), new BoolGenerator());
			TypeGenerators.Add(typeof(bool?), new BoolGenerator());
			TypeGenerators.Add(typeof(string), new StringGenerator());
			TypeGenerators.Add(typeof(byte[]), new ByteArrayGenerator());
			TypeGenerators.Add(typeof(ulong), new ULongGenerator());
			TypeGenerators.Add(typeof(ulong?), new ULongGenerator());
			TypeGenerators.Add(typeof(uint), new UIntGenerator());
			TypeGenerators.Add(typeof(uint?), new UIntGenerator());
			TypeGenerators.Add(typeof(Enum), new EnumGenerator());
		}

		public void AddGenerator(Type generatorType, FakeGenerator generator)
		{
			if (TypeGenerators.ContainsKey(generatorType)) TypeGenerators[generatorType] = generator;
			else TypeGenerators.Add(generatorType, generator);
		}

		public object GetValue(Type type)
		{
			if (type.IsEnum) return new EnumGenerator().Generate(type);

			return TypeGenerators.TryGetValue(type, out var faker) ? faker.Generate(type) : null;
		}

		public bool IsTypeFaked(Type type) => type.IsEnum || TypeGenerators.ContainsKey(type);
	}
}