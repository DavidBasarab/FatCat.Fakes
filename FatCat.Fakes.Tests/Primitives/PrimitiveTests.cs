using System.Linq;

namespace FatCat.Fakes.Tests.Primitives;

public abstract class PrimitiveTests<T>
{
    [Fact]
    public void CreateDifferentValues()
    {
        var values = Enumerable.Range(0, 13).Select(_ => Faker.Create<T>()).ToList();

        values.Distinct().Count().Should().BeGreaterThan(1);
    }
}
