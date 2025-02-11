namespace FatCat.Fakes.Tests;

public class GenericCustomGenerator
{
    [Fact]
    public void CanCreateAFakeOfAGenericClass()
    {
        var fake = Faker.Create(typeof(GenericClass<>));

        fake.Should().BeOfType(typeof(GenericClass<>));
    }

    [Fact]
    public void CanFactAnItemWithAGeneric()
    {
        var moduleFake = Faker.Create<ModuleItem>();

        moduleFake.Should().BeOfType(typeof(GenericClass<>));
    }
}

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
    public string Name { get; set; }

    public int Number { get; set; }
}

public class ItemOne : BaseItem
{
    public string ItemOneProperty { get; set; }
}

public class ItemTwo : BaseItem
{
    public int ItemTwoProperty { get; set; }
}
