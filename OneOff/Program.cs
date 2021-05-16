using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FatCat.Fakes;
using FatCat.Fakes.Tests.SpeedUp.Models;

namespace OneOff
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Console.WriteLine("Going to test how fast Faker is");

			var numberOfFakesToCreate = 12;

			var totalTime = TimeSpan.Zero;

			var tasks = new Task[numberOfFakesToCreate];

			for (var i = 0; i < numberOfFakesToCreate; i++)
			{
				tasks[i] = Task.Run(async () =>
									{
										var watch = Stopwatch.StartNew();

										var nasaFake = Faker.Create<Nasa>();

										watch.Stop();

										totalTime += watch.Elapsed;

										await Task.CompletedTask;
									});
			}

			Task.WaitAll(tasks);

			var avgMs = totalTime.TotalMilliseconds / 12;
			var avgTime = TimeSpan.FromMilliseconds(avgMs);

			Console.WriteLine($"Number Of Attempts <{numberOfFakesToCreate}> | Total Time {totalTime} | Average Time {avgTime} | Average Ms {avgMs}");
		}
	}
}