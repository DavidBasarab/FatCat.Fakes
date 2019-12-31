using System;
using System.Collections;
using System.Collections.Generic;
using Fasterflect;
using FatCat.Fakes.Generators;

namespace FatCat.Fakes
{
	public static class Faker
	{
		private static FakeFactory FakeFactory { get; } = FakeFactory.Instance;

		private static Random Random { get; } = new Random();

		public static T Create<T>(int? lengthOfList = null)
		{
			var fakeType = typeof(T);
			
			if (FakeFactory.IsTypeFaked(fakeType)) return (T)Create(fakeType);

			if (fakeType.IsArray) return (T)CreateArray(lengthOfList, fakeType);

			if (IsList<T>(fakeType)) return (T)CreateList(lengthOfList, fakeType);

			throw new ArgumentException($"A fake for type of {fakeType.FullName} is not supported");
		}

		public static object Create(Type fakeType) => FakeFactory.GetValue(fakeType);

		private static object CreateArray(int? lengthOfList, Type fakeType)
		{
			var length = lengthOfList ?? Random.Next(3, 9);

			var array = Array.CreateInstance(fakeType.GetElementType(), length);

			for (var i = 0; i < length; i++) array.SetValue(Create(fakeType.GetElementType()), i);

			return array;
		}

		private static object CreateList(int? lengthOfList, Type fakeType)
		{
			var itemType = fakeType.GetGenericArguments()[0];

			var genericListType = typeof(List<>);

			var subCombinedType = genericListType.MakeGenericType(itemType);
			var listAsInstance = Activator.CreateInstance(subCombinedType);

			var addMethod = listAsInstance.GetType().GetMethod("Add");

			var numberOfItems = lengthOfList ?? Random.Next(3, 9);

			for (var i = 0; i < numberOfItems; i++) addMethod.Invoke(listAsInstance, new[] { Create(itemType) });

			return listAsInstance;
		}

		private static bool IsList<T>(Type fakeType) => fakeType.IsGenericType && fakeType.Implements(typeof(IEnumerable));
	}
}