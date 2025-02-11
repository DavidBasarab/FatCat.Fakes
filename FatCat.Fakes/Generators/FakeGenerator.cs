using System;

namespace FatCat.Fakes.Generators
{
    public abstract class FakeGenerator
    {
        private static Random random;

        protected static Random Random
        {
            get { return random ??= new Random(); }
        }

        public abstract object Generate(Type typeToGenerate);
    }
}
