namespace FatCat.Fakes.Tests;

public class GenericClass<T> : ModuleItem
	where T : BaseItem
{
	public T Value { get; set; }
}
