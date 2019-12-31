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

		public static T Create<T>()
		{
			var fakeType = typeof(T);

			if (fakeType.IsGenericType && fakeType.Implements(typeof(IList)))
			{
				var itemType = fakeType.GetGenericArguments()[0];

				var genericListType = typeof(List<>);

				var subCombinedType = genericListType.MakeGenericType(itemType);
				var listAsInstance = Activator.CreateInstance(subCombinedType);

				var addMethod = listAsInstance.GetType().GetMethod("Add");

				var numberOfItems = Random.Next(3, 9);

				for (int i = 0; i < numberOfItems; i++) { addMethod.Invoke(listAsInstance, new[] { Create(itemType) }); }

				return (T)listAsInstance;
			}

			return (T)Create(fakeType);
		}

		public static object Create(Type fakeType) => FakeFactory.GetValue(fakeType);
	}
}