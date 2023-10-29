using System;

namespace FatCat.Fakes.Generators
{
    internal class ByteArrayGenerator : FakeGenerator
    {
        public override object Generate(Type typeToGenerate)
        {
            var length = Random.Next(1, 4);

            var array = new byte[length];

            var byteGenerator = new ByteGenerator();

            for (var i = 0; i < length; i++)
                array[i] = (byte)byteGenerator.Generate(typeToGenerate);

            return array;
        }
    }
}
