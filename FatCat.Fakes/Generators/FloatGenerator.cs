using System;

namespace FatCat.Fakes.Generators
{
    internal class FloatGenerator : FakeGenerator
    {
        public override object Generate(Type typeToGenerate)
        {
            return (float)Random.NextDouble();
        }
    }
}
