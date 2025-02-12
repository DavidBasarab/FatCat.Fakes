using System;
using System.Collections.Generic;
using System.Linq;

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
        return type.GetMethods().Any(m => m.Name == "<Clone>$");
    }
}
