using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
