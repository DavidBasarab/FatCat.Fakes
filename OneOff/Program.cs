using System;
using FatCat.Fakes;

namespace OneOff
{
	public class SubObject
	{
		public DateTime Date { get; set; }

		public int First { get; set; }

		public int Second { get; set; }
	}

	public class TestFakingItem
	{
		public int SomeNumber { get; set; }

		public string SomeString { get; set; }

		public SubObject SubObject { get; set; }
	}

	internal class Program
	{
		private static void Main(string[] args) { Faker.PlayWithIdea<TestFakingItem>(p => p.SubObject.Date, p => p.SomeNumber); }
	}
}