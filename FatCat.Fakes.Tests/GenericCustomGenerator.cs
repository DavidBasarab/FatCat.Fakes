namespace FatCat.Fakes.Tests;

public class GenericCustomGenerator
{
    [Fact]
    public void CanCreateAFakeOfAGenericClass()
    {
        var fake = Faker.Create(typeof(GenericClass<>));

        fake.GetType().GetGenericTypeDefinition().Should().Be(typeof(GenericClass<>));
    }

    [Fact]
    public void CanFactAnItemWithAGeneric()
    {
        var moduleFake = Faker.Create<ModuleItem>();

        moduleFake.GetType().GetGenericTypeDefinition().Should().Be(typeof(GenericClass<>));
    }
}
