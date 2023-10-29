using System;

namespace FatCat.Fakes.Tests.SpeedUp.Models
{
    public class Computer : Equipment
    {
        public int NumberOfDisks { get; set; }

        public float ProcessorSpeed { get; set; }

        public string ProcessorType { get; set; }

        public int Ram { get; set; }

        public DateTime TimeOfBoot { get; set; }
    }
}
