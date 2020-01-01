using System;
using System.Collections.Generic;
using Xunit;

namespace FatCat.Fakes.Tests
{
	public class MyClass
	{
		public DateTime CreatedDate { get; set; }

		public string Name { get; set; }

		public int NumberOfTimes { get; set; }
	}

	public class ForDoc
	{
		public interface IDoSomeWork 
		{
			void DoWork();

			int Result { get; set; }

			string Name { get; }
		}

		public class FirstWorker : IDoSomeWork
		{
			public void DoWork() 
			{
				// Do first work
			}

			public int Result { get; set; }

			public string Name => "FirstWorker";
		}

		public class SecondWorker : IDoSomeWork
		{
			public void DoWork() 
			{
				// Do first work
			}

			public int Result { get; set; }

			public string Name => "SecondWorker";
		}
		
		public class MultiLevelObject 
		{
			public MyClass ObjectOne { get; set; }

			public Guid Id { get; set; }
		}
		
		[Fact]
		public void DoesNotMatter()
		{
			var someNumber = Faker.Create<int>();

			// create simple one level class
			var myClass = Faker.Create<MyClass>();

			var json = System.Text.Json.JsonSerializer.Serialize(myClass);
		}
	}
}