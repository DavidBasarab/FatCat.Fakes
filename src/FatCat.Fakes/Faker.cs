using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

			return (T)Create(fakeType, lengthOfList);
		}

		public static object Create(Type fakeType, int? lengthOfList = null)
		{
			if (FakeFactory.IsTypeFaked(fakeType)) return FakeFactory.GetValue(fakeType);

			if (fakeType.IsArray) return CreateArray(lengthOfList, fakeType);

			if (IsList(fakeType)) return CreateList(lengthOfList, fakeType);

			return CreateInstance(fakeType);
		}

		private static object CreateArray(int? lengthOfList, Type fakeType)
		{
			var length = lengthOfList ?? Random.Next(3, 9);

			var array = Array.CreateInstance(fakeType.GetElementType(), length);

			for (var i = 0; i < length; i++) array.SetValue(Create(fakeType.GetElementType()), i);

			return array;
		}

		private static object CreateInstance(Type fakeType)
		{
			var typeToCreate = fakeType;

			if (fakeType.IsAbstract || fakeType.IsInterface) typeToCreate = FindImplementingType(fakeType);

			var instance = Activator.CreateInstance(typeToCreate);

			var properties = new List<PropertyInfo>(typeToCreate.GetProperties());

			foreach (var propertyInfo in properties.Where(i => i.CanWrite)) propertyInfo.SetValue(instance, Create(propertyInfo.PropertyType));

			return instance;
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

		private static Type FindImplementingType(Type fakeType)
		{
			var assembly = fakeType.Assembly;

			var types = assembly.GetTypes()
								.Where(i => i.IsClass && !i.IsAbstract && (i.IsSubclassOf(fakeType) || i.Implements(fakeType)))
								.ToList();

			var typeIndex = Random.Next(types.Count);

			return types[typeIndex];
		}

		private static bool IsList(Type fakeType) => fakeType.IsGenericType && fakeType.Implements(typeof(IEnumerable));
	}
}