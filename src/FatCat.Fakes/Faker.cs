﻿using System;
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

		public static T Create<T>(Action<T> afterCreate = null, int? length = null)
		{
			var fakeType = typeof(T);

			var item = (T)Create(fakeType, length: length);

			afterCreate?.Invoke(item);

			return item;
		}

		public static object Create(Type fakeType, Action<object> afterCreate = null, int? length = null)
		{
			if (FakeFactory.IsTypeFaked(fakeType)) return FakeFactory.GetValue(fakeType);

			if (fakeType.IsArray) return CreateArray(length, fakeType);

			if (IsList(fakeType))
			{
				if (IsDictionary(fakeType)) return CreateDictionary(length, fakeType);

				return CreateList(length, fakeType);
			}

			var item = CreateInstance(fakeType);

			afterCreate?.Invoke(item);

			return item;
		}

		public static int RandomInt(int maxValue) => RandomInt(null, maxValue);

		public static int RandomInt(int? minValue = null, int? maxValue = null)
		{
			if (minValue.HasValue && maxValue.HasValue) return Random.Next(minValue.Value, maxValue.Value);

			if (maxValue.HasValue) return Random.Next(maxValue.Value);

			return Random.Next();
		}

		public static long RandomLong(int? minValue = null, int? maxValue = null)
		{
			if (minValue.HasValue && maxValue.HasValue) return Random.Next(minValue.Value, maxValue.Value);

			return (long)Random.NextDouble();
		}

		public static long RandomLong(int maxValue) => RandomLong(null, maxValue);

		private static object CreateArray(int? lengthOfList, Type fakeType)
		{
			var length = lengthOfList ?? Random.Next(3, 9);

			var array = Array.CreateInstance(fakeType.GetElementType(), length);

			for (var i = 0; i < length; i++) array.SetValue(Create(fakeType.GetElementType()), i);

			return array;
		}

		private static object CreateDictionary(int? lengthOfList, Type fakeType)
		{
			var keyType = fakeType.GetGenericArguments()[0];
			var valueType = fakeType.GetGenericArguments()[1];

			var genericDictionary = typeof(Dictionary<,>);

			var finalDictionaryType = genericDictionary.MakeGenericType(keyType, valueType);

			var dictionary = Activator.CreateInstance(finalDictionaryType);

			var addMethod = dictionary.GetType().GetMethod("Add");

			var numberOfItems = lengthOfList ?? Random.Next(3, 9);

			for (var i = 0; i < numberOfItems; i++)
			{
				try { addMethod.Invoke(dictionary, new[] { Create(keyType), Create(valueType) }); }
				catch (ArgumentException)
				{
					// Added the same key, skipping entry
				}
			}

			return dictionary;
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

		private static bool IsDictionary(Type fakeType) => fakeType.IsGenericType && fakeType.Implements(typeof(IDictionary<,>));

		private static bool IsList(Type fakeType) => fakeType.IsGenericType && fakeType.Implements(typeof(IEnumerable));

		public static string RandomString() { return Create<string>(); }
	}
}