using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Fasterflect;
using FatCat.Fakes.Generators;

namespace FatCat.Fakes
{
	public static class Faker
	{
		internal static FakeFactory FakeFactory { get; } = FakeFactory.Instance;

		private static Random Random { get; } = new Random();

		public static void AddGenerator(Type generatorType, FakeGenerator generator) => FakeFactory.Instance.AddGenerator(generatorType, generator);

		public static T Create<T>(params Expression<Func<T, object>>[] propertiesToIgnore) => Create(i => { }, null, propertiesToIgnore);

		public static T Create<T>(Action<T> afterCreate, params Expression<Func<T, object>>[] propertiesToIgnore) => Create(afterCreate, null, propertiesToIgnore);

		public static T Create<T>(int? length, params Expression<Func<T, object>>[] propertiesToIgnore) => Create(i => { }, length, propertiesToIgnore);

		// ReSharper disable once MemberCanBePrivate.Global
		public static T Create<T>(Action<T> afterCreate, int? length, IEnumerable<Expression<Func<T, object>>> propertiesToIgnore)
		{
			var fakeType = typeof(T);

			var item = (T)Create(fakeType, length: length);

			foreach (var expression in propertiesToIgnore)
			{
				MemberExpression memberExpression;

				if (expression.Body is UnaryExpression unaryExpression) memberExpression = (MemberExpression)unaryExpression.Operand;
				else memberExpression = (MemberExpression)expression.Body;

				var propertyInfo = (PropertyInfo)memberExpression.Member;

				if (propertyInfo.DeclaringType == item.GetType()) propertyInfo.SetValue(item, null);
				else
				{
					var parts = memberExpression.ToString().Split('.').Skip(1).ToList();

					object subValue = item;

					for (var i = 0; i < parts.Count - 1; i++)
					{
						var expressionPart = parts[i];

						var subPropertyInfo = subValue.GetType().GetProperty(expressionPart);

						if (subPropertyInfo != null) subValue = subPropertyInfo.GetValue(subValue);
					}

					if (subValue != null) propertyInfo.SetValue(subValue, null);
				}
			}

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

		public static void PlayWithIdea<T>(params Expression<Func<T, object>>[] items) where T : class
		{
			foreach (var property in items)
			{
				var lambda = property;
				MemberExpression memberExpression;

				if (lambda.Body is UnaryExpression unaryExpression) memberExpression = (MemberExpression)unaryExpression.Operand;
				else memberExpression = (MemberExpression)lambda.Body;

				var propertyInfo = (PropertyInfo)memberExpression.Member;

				Console.WriteLine($"  PropertyInfo.FullName := {propertyInfo.Name} | Type := {propertyInfo.PropertyType} | DeclaringType := {propertyInfo.DeclaringType}");
			}
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

		public static string RandomString(string prefix = null, int? length = null)
		{
			var stringGenerator = new StringGenerator();

			var randomString = length.HasValue ? stringGenerator.Generate(length.Value) : (string)stringGenerator.Generate(typeof(string));

			return $"{prefix}{randomString}";
		}

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
	}
}