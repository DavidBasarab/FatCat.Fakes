using System;

namespace OneOff.Models
{
    public abstract class Equipment
    {
        public DateTime DateAcquired { get; set; }

        public Guid Id { get; set; }

        public string Manufacture { get; set; }

        public string Name { get; set; }

        public bool OnLoan { get; set; }

        public string Owner { get; set; }
    }
}
