using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FatCat.Fakes;

internal static class DynamicRecordCreator
{
    public static T CreateRecord<T>()
    {
        var item = CreateRecord(typeof(T));

        return item == null ? default : (T)item;
    }

    public static object CreateRecord(Type recordType)
    {
        var recordParameters = recordType
            .GetProperties()
            .ToDictionary(prop => prop.Name, prop => Faker.Create(prop.PropertyType));

        return CreateRecord(recordType, recordParameters);
    }

    public static object CreateRecord(Type recordType, Dictionary<string, object> propertyValues)
    {
        var instance = Activator.CreateInstance(recordType, propertyValues.Values.ToArray());

        return instance;
    }

    public static bool IsRecordType(this Type type)
    {
        if (!type.IsClass && !type.IsValueType)
        {
            return false; // Must be a class or struct
        }

        // Check if ToString is overridden
        var toStringMethod = type.GetMethod("ToString", Type.EmptyTypes);
        var hasToStringOverride = toStringMethod?.DeclaringType == type;

        // Check if Equals is overridden
        var equalsMethod = type.GetMethod("Equals", new[] { typeof(object) });
        var hasEqualsOverride = equalsMethod?.DeclaringType == type;

        // Check if any property has an 'init' accessor (unique to records)
        var hasInitOnlyProperties = type.GetProperties()
            .Any(
                prop =>
                    prop.SetMethod
                        ?.ReturnParameter?.GetRequiredCustomModifiers()
                        .Contains(typeof(IsExternalInit)) == true
            );

        return (hasToStringOverride && hasEqualsOverride) || hasInitOnlyProperties;
    }
}
