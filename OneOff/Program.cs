using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public static object CreateRecord(Type recordType, Dictionary<string, object> properties)
    {
        if (properties == null || properties.Count == 0)
        {
            throw new ArgumentException("At least one property is required.");
        }

        var assemblyName = recordType.Assembly.GetName();

        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            assemblyName,
            AssemblyBuilderAccess.Run
        );

        var moduleBuilder = assemblyBuilder.DefineDynamicModule(recordType.Name + "Module");

        var typeBuilder = moduleBuilder.DefineType(
            recordType.Name,
            TypeAttributes.Public | TypeAttributes.Sealed,
            typeof(object)
        );

        var fields = new List<FieldBuilder>();
        var propertyBuilders = new List<PropertyBuilder>();
        var constructorParamTypes = new List<Type>();

        foreach (var property in properties)
        {
            var propertyName = property.Key;
            var propertyType = property.Value.GetType();

            // Define private field
            var fieldBuilder = typeBuilder.DefineField(
                "_" + propertyName,
                propertyType,
                FieldAttributes.Private
            );

            fields.Add(fieldBuilder);

            // Define property
            var propertyBuilder = typeBuilder.DefineProperty(
                propertyName,
                PropertyAttributes.HasDefault,
                propertyType,
                null
            );

            propertyBuilders.Add(propertyBuilder);

            // Define getter
            var getMethodBuilder = typeBuilder.DefineMethod(
                "get_" + propertyName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                propertyType,
                Type.EmptyTypes
            );

            var getIL = getMethodBuilder.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getIL.Emit(OpCodes.Ret);

            // Define setter
            var setMethodBuilder = typeBuilder.DefineMethod(
                "set_" + propertyName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                null,
                new[] { propertyType }
            );

            var setIL = setMethodBuilder.GetILGenerator();
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, fieldBuilder);
            setIL.Emit(OpCodes.Ret);

            // Attach getter and setter to property
            propertyBuilder.SetGetMethod(getMethodBuilder);
            propertyBuilder.SetSetMethod(setMethodBuilder);

            // Collect constructor parameters
            constructorParamTypes.Add(propertyType);
        }

        // Define constructor
        var constructorBuilder = typeBuilder.DefineConstructor(
            MethodAttributes.Public,
            CallingConventions.Standard,
            constructorParamTypes.ToArray()
        );

        var ctorIL = constructorBuilder.GetILGenerator();
        ctorIL.Emit(OpCodes.Ldarg_0);
        ctorIL.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)); // Call base constructor

        var index = 1;

        foreach (var field in fields)
        {
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg, index++);
            ctorIL.Emit(OpCodes.Stfld, field);
        }

        ctorIL.Emit(OpCodes.Ret);

        // Create type
        var dynamicType = typeBuilder.CreateType();

        // Instantiate and return the new record
        var constructorArgs = new object[properties.Count];
        properties.Values.CopyTo(constructorArgs, 0);

        return Activator.CreateInstance(dynamicType, constructorArgs);
    }
}

public record TestingRecord(int Number, string Name);

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            var recordType = typeof(TestingRecord);

            for (int i = 0; i < 50; i++)
            {
                var recordParameters = new Dictionary<string, object>();

                foreach (var prop in recordType.GetProperties())
                {
                    recordParameters.Add(prop.Name, Faker.Create(prop.PropertyType));
                }

                var watch = Stopwatch.StartNew();

                var item = DynamicRecordCreator.CreateRecord(recordType, recordParameters);

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
