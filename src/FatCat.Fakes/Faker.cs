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

			if (IsList<T>(fakeType)) return (T)CreateList(lengthOfList, fakeType);

			return (T)Create(fakeType);
		}

		public static object Create(Type fakeType) => FakeFactory.GetValue(fakeType);

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

		private static bool IsList<T>(Type fakeType)
		{
			return fakeType.IsGenericType && (fakeType.Implements(typeof(IList)) || fakeType.Implements(typeof(IEnumerable)));
		}
	}
}