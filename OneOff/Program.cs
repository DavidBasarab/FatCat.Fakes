using System;
using System.Diagnostics;
using FatCat.Fakes;
using FatCat.Toolkit.Console;
using Newtonsoft.Json;

namespace OneOff;

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            for (var i = 0; i < 50; i++)
            {
                var watch = Stopwatch.StartNew();

                var item = Faker.Create<TestingMongoObject>();

                watch.Stop();

                ConsoleLog.WriteGreen($"{JsonConvert.SerializeObject(item)} | {watch.Elapsed}");
            }
        }
        catch (Exception ex)
        {
            ConsoleLog.WriteException(ex);
        }
    }
}
