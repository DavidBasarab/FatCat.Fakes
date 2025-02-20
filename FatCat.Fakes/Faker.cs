﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Fasterflect;
using FatCat.Fakes.Generators;

namespace FatCat.Fakes
{
    public static class Faker
    {
        public static Random Random { get; } = new Random();

        internal static FakeFactory FakeFactory { get; } = FakeFactory.Instance;

        private static ConcurrentDictionary<string, List<Type>> CacheOfImplementingTypes { get; } =
            new();

        public static void AddGenerator(Type generatorType, FakeGenerator generator)
        {
            FakeFactory.Instance.AddGenerator(generatorType, generator);
        }

        public static T Create<T>()
        {
            return Create<T>(i => { }, null);
        }

        public static T Create<T>(Action<T> afterCreate)
        {
            return Create(afterCreate, null);
        }

        public static T Create<T>(int? length)
        {
            return Create<T>(i => { }, length);
        }

        public static T Create<T>(params Expression<Func<T, object>>[] propertiesToIgnore)
        {
            return Create(i => { }, null, propertiesToIgnore);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static T Create<T>(
            Action<T> afterCreate,
            int? length,
            IEnumerable<Expression<Func<T, object>>> propertiesToIgnore = null
        )
        {
            var fakeType = typeof(T);

            var item = (T)Create(fakeType, length: length);

            if (propertiesToIgnore != null)
            {
                foreach (var expression in propertiesToIgnore)
                {
                    MemberExpression memberExpression;

                    if (expression.Body is UnaryExpression unaryExpression)
                    {
                        memberExpression = (MemberExpression)unaryExpression.Operand;
                    }
                    else
                    {
                        memberExpression = (MemberExpression)expression.Body;
                    }

                    var propertyInfo = (PropertyInfo)memberExpression.Member;

                    if (propertyInfo.DeclaringType == item.GetType())
                    {
                        propertyInfo.SetValue(item, null);
                    }
                    else
                    {
                        var parts = memberExpression.ToString().Split('.').Skip(1).ToList();

                        object subValue = item;

                        for (var i = 0; i < parts.Count - 1; i++)
                        {
                            var expressionPart = parts[i];

                            var subPropertyInfo = subValue.GetType().GetProperty(expressionPart);

                            if (subPropertyInfo != null)
                            {
                                subValue = subPropertyInfo.GetValue(subValue);
                            }
                        }

                        if (subValue != null)
                        {
                            propertyInfo.SetValue(subValue, null);
                        }
                    }
                }
            }

            afterCreate?.Invoke(item);

            return item;
        }

        public static object Create(
            Type fakeType,
            Action<object> afterCreate = null,
            int? length = null
        )
        {
            if (FakeFactory.IsTypeFaked(fakeType))
            {
                return FakeFactory.GetValue(fakeType);
            }

            if (fakeType.IsRecordType())
            {
                return DynamicRecordCreator.CreateRecord(fakeType);
            }

            if (fakeType.IsArray)
            {
                return CreateArray(length, fakeType);
            }

            if (IsList(fakeType))
            {
                if (IsDictionary(fakeType))
                {
                    return CreateDictionary(length, fakeType);
                }

                return CreateList(length, fakeType);
            }

            var item = CreateInstance(fakeType);

            afterCreate?.Invoke(item);

            return item;
        }

        public static byte[] RandomBytes(int length)
        {
            var bytes = new byte[length];

            Random.NextBytes(bytes);

            return bytes;
        }

        public static Color RandomColor()
        {
            return Color.FromArgb(RandomInt(0, 256), RandomInt(0, 256), RandomInt(0, 256));
        }

        public static DateTime RandomDateTime()
        {
            return Create<DateTime>();
        }

        public static int RandomInt(int maxValue)
        {
            return RandomInt(null, maxValue);
        }

        public static int RandomInt(int? minValue = null, int? maxValue = null)
        {
            if (minValue.HasValue && maxValue.HasValue)
            {
                return Random.Next(minValue.Value, maxValue.Value);
            }

            if (maxValue.HasValue)
            {
                return Random.Next(maxValue.Value);
            }

            return Random.Next();
        }

        public static long RandomLong(int? minValue = null, int? maxValue = null)
        {
            if (minValue.HasValue && maxValue.HasValue)
            {
                return Random.Next(minValue.Value, maxValue.Value);
            }

            return (long)Random.NextDouble();
        }

        public static long RandomLong(int maxValue)
        {
            return RandomLong(null, maxValue);
        }

        public static string RandomString(string prefix = null, int? length = null)
        {
            var stringGenerator = new StringGenerator();

            var randomString = length.HasValue
                ? stringGenerator.Generate(length.Value)
                : (string)stringGenerator.Generate(typeof(string));

            return $"{prefix}{randomString}";
        }

        private static object CreateArray(int? lengthOfList, Type fakeType)
        {
            var length = lengthOfList ?? Random.Next(3, 9);

            var array = Array.CreateInstance(fakeType.GetElementType(), length);

            for (var i = 0; i < length; i++)
            {
                array.SetValue(Create(fakeType.GetElementType()), i);
            }

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
                try
                {
                    addMethod.Invoke(dictionary, new[] { Create(keyType), Create(valueType) });
                }
                catch (ArgumentException)
                {
                    // Added the same key, skipping entry
                }
            }

            return dictionary;
        }

        private static object CreateGenericType(Type typeToCreate)
        {
            var genericArguments = typeToCreate.GetGenericArguments();

            var genericType = genericArguments[0];

            var typeImplementingGeneric = FindImplementingType(genericType.BaseType);

            var combinedType = typeToCreate.MakeGenericType(typeImplementingGeneric);

            return Activator.CreateInstance(combinedType);
        }

        private static object CreateInstance(Type fakeType)
        {
            var typeToCreate = fakeType;

            if (fakeType.IsAbstract || fakeType.IsInterface)
            {
                typeToCreate = FindImplementingType(fakeType);
            }

            if (string.IsNullOrEmpty(fakeType.FullName))
            {
                typeToCreate = FindImplementingType(fakeType.BaseType);
            }

            if (DoesNotHaveParameterLessConstructor(typeToCreate))
            {
                return null;
            }

            var instance = typeToCreate.IsGenericType
                ? CreateGenericType(typeToCreate)
                : Activator.CreateInstance(typeToCreate);

            var properties = new List<PropertyInfo>(instance.GetType().GetProperties());

            foreach (var propertyInfo in properties.Where(i => i.CanWrite))
            {
                var value = Create(propertyInfo.PropertyType);

                propertyInfo.SetValue(instance, value);
            }

            return instance;
        }

        private static object CreateList(int? lengthOfList, Type fakeType)
        {
            var itemType = fakeType.GetGenericArguments()[0];

            var genericListType = typeof(List<>);

            var subCombinedType = genericListType.MakeGenericType(itemType);
            var listAsInstance = Activator.CreateInstance(subCombinedType);

            var addMethod = listAsInstance.GetType().GetMethod("Add");

            var numberOfItems = lengthOfList ?? Random.Next(1, 3);

            for (var i = 0; i < numberOfItems; i++)
            {
                addMethod.Invoke(listAsInstance, new[] { Create(itemType) });
            }

            return listAsInstance;
        }

        private static bool DoesNotHaveParameterLessConstructor(Type fakeType)
        {
            return fakeType.GetConstructor(Type.EmptyTypes) == null;
        }

        private static Type FindImplementingType(Type fakeType)
        {
            var foundTypes = CacheOfImplementingTypes.TryGetValue(fakeType.FullName, out var types);

            if (!foundTypes)
            {
                var assembly = fakeType.Assembly;

                types = assembly
                    .GetTypes()
                    .Where(
                        i =>
                            i.IsClass
                            && !i.IsAbstract
                            && (i.IsSubclassOf(fakeType) || i.Implements(fakeType))
                    )
                    .ToList();

                CacheOfImplementingTypes.TryAdd(fakeType.FullName, types);
            }

            var typeIndex = Random.Next(types.Count);

            return types[typeIndex];
        }

        private static bool IsDictionary(Type fakeType)
        {
            return fakeType.IsGenericType && fakeType.Implements(typeof(IDictionary<,>));
        }

        private static bool IsList(Type fakeType)
        {
            return fakeType.IsGenericType && fakeType.Implements(typeof(IEnumerable));
        }
    }
}
