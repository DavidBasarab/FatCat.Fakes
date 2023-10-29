using System;
using System.Collections.Concurrent;

namespace FatCat.Fakes.Generators
{
internal class FakeFactory
{
	private static readonly Lazy<FakeFactory> instance = new Lazy<FakeFactory>(
																				() => new FakeFactory()
																			);

	public static FakeFactory Instance => instance.Value;

	private ConcurrentDictionary<Type, FakeGenerator> TypeGenerators { get; } =
		new ConcurrentDictionary<Type, FakeGenerator>();

	private FakeFactory()
	{
		TypeGenerators.TryAdd(typeof(int), new IntGenerator());
		TypeGenerators.TryAdd(typeof(int?), new IntGenerator());
		TypeGenerators.TryAdd(typeof(DateTime), new DateTimeGenerator());
		TypeGenerators.TryAdd(typeof(DateTime?), new DateTimeGenerator());
		TypeGenerators.TryAdd(typeof(TimeSpan), new TimespanGenerator());
		TypeGenerators.TryAdd(typeof(TimeSpan?), new TimespanGenerator());
		TypeGenerators.TryAdd(typeof(double), new DoubleGenerator());
		TypeGenerators.TryAdd(typeof(double?), new DoubleGenerator());
		TypeGenerators.TryAdd(typeof(byte), new ByteGenerator());
		TypeGenerators.TryAdd(typeof(byte?), new ByteGenerator());
		TypeGenerators.TryAdd(typeof(short), new ShortGenerator());
		TypeGenerators.TryAdd(typeof(short?), new ShortGenerator());
		TypeGenerators.TryAdd(typeof(ushort), new UshortGenerator());
		TypeGenerators.TryAdd(typeof(ushort?), new UshortGenerator());
		TypeGenerators.TryAdd(typeof(float), new FloatGenerator());
		TypeGenerators.TryAdd(typeof(float?), new FloatGenerator());
		TypeGenerators.TryAdd(typeof(long), new LongGenerator());
		TypeGenerators.TryAdd(typeof(long?), new LongGenerator());
		TypeGenerators.TryAdd(typeof(Guid), new GuidGenerator());
		TypeGenerators.TryAdd(typeof(Guid?), new GuidGenerator());
		TypeGenerators.TryAdd(typeof(bool), new BoolGenerator());
		TypeGenerators.TryAdd(typeof(bool?), new BoolGenerator());
		TypeGenerators.TryAdd(typeof(string), new StringGenerator());
		TypeGenerators.TryAdd(typeof(byte[]), new ByteArrayGenerator());
		TypeGenerators.TryAdd(typeof(ulong), new ULongGenerator());
		TypeGenerators.TryAdd(typeof(ulong?), new ULongGenerator());
		TypeGenerators.TryAdd(typeof(uint), new UIntGenerator());
		TypeGenerators.TryAdd(typeof(uint?), new UIntGenerator());
		TypeGenerators.TryAdd(typeof(Enum), new EnumGenerator());
	}

	public void AddGenerator(Type generatorType, FakeGenerator generator)
	{
		if (TypeGenerators.ContainsKey(generatorType)) { TypeGenerators[generatorType] = generator; }
		else { TypeGenerators.TryAdd(generatorType, generator); }
	}

	public object GetValue(Type type)
	{
		if (type.IsEnum) { return new EnumGenerator().Generate(type); }

		return TypeGenerators.TryGetValue(type, out var faker) ? faker.Generate(type) : null;
	}

	public bool IsTypeFaked(Type type) => type.IsEnum || TypeGenerators.ContainsKey(type);
}
}