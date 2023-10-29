using System;

namespace FatCat.Fakes.Generators
{
    internal class DateTimeGenerator : FakeGenerator
    {
        public override object Generate(Type typeToGenerate)
        {
            var now = DateTime.Now;

            var randomDays = Random.Next(-1500, 7500);

            now = now.AddDays(randomDays);

            now = AddRandomTimeSpans(now);

            return now.AddDays(randomDays);
        }

        private static DateTime AddRandomTimeSpans(DateTime now)
        {
            var timeSpanGenerator = new TimespanGenerator();

            for (var i = 0; i < Random.Next(3, 11); i++)
                now += (TimeSpan)timeSpanGenerator.Generate(typeof(TimeSpan));

            return now;
        }
    }
}
