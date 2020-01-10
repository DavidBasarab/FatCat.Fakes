using System;
using System.Collections.Generic;
using System.Linq;
using FatCat.Fakes;

namespace OneOff
{
    public class SubObject
    {
        public int First { get; set; }

        public int Second { get; set; }

        public DateTime Date { get; set; }
    }
    
    public class TestFakingItem
    {
        public int SomeNumber { get; set; }

        public string SomeString { get; set; }

        public SubObject SubObject { get; set; }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            Faker.PlayWithIdea<TestFakingItem>(p => p.SubObject.Date);
        }
    }
}
