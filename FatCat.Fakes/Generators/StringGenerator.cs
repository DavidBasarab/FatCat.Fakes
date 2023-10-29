using System;
using System.Text;

namespace FatCat.Fakes.Generators
{
    internal class StringGenerator : FakeGenerator
    {
        public override object Generate(Type typeToGenerate)
        {
            var length = Random.Next(8, 37);

            return Generate(length);
        }

        public string Generate(int length)
        {
            var builder = new StringBuilder();

            var characters = Guid.NewGuid().ToString().Replace("-", string.Empty);

            var characterIndex = 0;

            for (var i = 0; i < length; i++)
            {
                builder.Append(characters[characterIndex]);

                characterIndex++;

                if (characterIndex >= characters.Length)
                    characterIndex = 0;
            }

            return builder.ToString();
        }
    }
}
