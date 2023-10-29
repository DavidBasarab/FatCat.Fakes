using System;
using System.Diagnostics;
using FatCat.Fakes;
using OneOff.Models;

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

internal class Program
{
    private static void Main(string[] args)
    {
        var moduleFake = Faker.Create<ModuleItem>();

        Console.WriteLine($"ModuleFake Type {moduleFake.GetType().FullName}");
    }
}
