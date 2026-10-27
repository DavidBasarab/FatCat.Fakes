namespace FatCat.Fakes.Tests.Primitives
{
    public abstract class PrimitiveTests<T>
    {
        [Fact]
        public void EachItemWillBeRandom()
        {
            var previousValue = default(T);

            for (var i = 0; i < 13; i++)
            {
                var currentValue = Faker.Create<T>();

                ((object)currentValue).Should().Not.Be(previousValue);

                previousValue = currentValue;
            }
        }
    }
}
