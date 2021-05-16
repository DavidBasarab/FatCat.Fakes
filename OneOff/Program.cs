using System;
using System.Diagnostics;
using FatCat.Fakes;
using FatCat.Fakes.Tests.SpeedUp.Models;

namespace OneOff
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Console.WriteLine("Going to test how fast Faker is");

			var watch = Stopwatch.StartNew();

			var nasaFake = Faker.Create<Nasa>();
			
			watch.Stop();
			
			Console.WriteLine($"Took <{watch.Elapsed}> to create | TotalMS <{watch.Elapsed.TotalMilliseconds}>");
		}
	}
}