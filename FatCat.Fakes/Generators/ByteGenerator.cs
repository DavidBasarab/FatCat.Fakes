using System;

namespace FatCat.Fakes.Generators
{
    internal class ByteGenerator : FakeGenerator
    {
        public override object Generate(Type typeToGenerate)
        {
            return (byte)Random.Next(byte.MinValue, byte.MaxValue);
        }
    }
}
