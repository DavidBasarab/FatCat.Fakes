using System;

namespace FatCat.Fakes.Generators
{
    internal class ShortGenerator : FakeGenerator
    {
        public override object Generate(Type typeToGenerate)
        {
            return (short)Random.Next(short.MinValue, short.MaxValue);
        }
    }
}
