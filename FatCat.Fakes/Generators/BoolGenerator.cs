using System;

namespace FatCat.Fakes.Generators
{
    internal class BoolGenerator : FakeGenerator
    {
        public override object Generate(Type typeToGenerate) => Random.Next() % 2 == 0;
    }
}
