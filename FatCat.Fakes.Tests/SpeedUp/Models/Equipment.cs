using System;

namespace FatCat.Fakes.Tests.SpeedUp.Models
{
    public abstract class Equipment
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Owner { get; set; }

        public bool OnLoan { get; set; }

        public DateTime DateAcquired { get; set; }

        public string Manufacture { get; set; }
    }
}
