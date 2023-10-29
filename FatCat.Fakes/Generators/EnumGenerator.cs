using System;

namespace FatCat.Fakes.Generators
{
    internal class EnumGenerator : FakeGenerator
    {
        public override object Generate(Type typeToGenerate)
        {
            var values = Enum.GetValues(typeToGenerate);

            var index = Random.Next(values.Length);

            return values.GetValue(index);
        }
    }
}
