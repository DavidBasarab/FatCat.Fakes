using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using FatCat.Fakes;
using FatCat.Toolkit.Console;
using Newtonsoft.Json;

namespace OneOff;

public abstract class ModuleItem
{
    public string Room { get; set; }

    public int Times { get; set; }
}

public class GenericClass<T> : ModuleItem
    where T : BaseItem
{
    public T Value { get; set; }
}

public abstract class BaseItem
{
    // public string Name { get; set; }
    //
    // public int Number { get; set; }
}

public class ItemOne : BaseItem
{
    // public string ItemOneProperty { get; set; }
}

public class ItemTwo : BaseItem
{
    // public int ItemTwoProperty { get; set; }
}

public class DynamicRecordCreator
{
    public static T CreateRecord<T>()
    {
        var item = CreateRecord(typeof(T));

        ConsoleLog.WriteGreen($"{item.GetType().FullName} | {typeof(T).FullName}");

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
        if (recordType == null)
            throw new ArgumentNullException(nameof(recordType));
        if (propertyValues == null || propertyValues.Count == 0)
            throw new ArgumentException("At least one property value is required.");

        // Ensure that the type has the expected properties
        PropertyInfo[] properties = recordType.GetProperties();
        foreach (var property in propertyValues)
        {
            if (
                !properties.Any(
                    p => p.Name == property.Key && p.PropertyType == property.Value.GetType()
                )
            )
                throw new ArgumentException(
                    $"Property '{property.Key}' with type '{property.Value.GetType()}' is not found in the specified type."
                );
        }

        // Create an instance of the record type using its constructor
        object instance = Activator.CreateInstance(recordType, propertyValues.Values.ToArray());

        return instance;
    }
}

public record TestingRecord(int Number, string Name);

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            for (var i = 0; i < 50; i++)
            {
                var watch = Stopwatch.StartNew();

                var item = DynamicRecordCreator.CreateRecord<TestingRecord>();

                watch.Stop();

                ConsoleLog.WriteGreen($"{JsonConvert.SerializeObject(item)} | {watch.Elapsed}");
            }
        }
        catch (Exception ex)
        {
            ConsoleLog.WriteException(ex);
        }
    }
}
