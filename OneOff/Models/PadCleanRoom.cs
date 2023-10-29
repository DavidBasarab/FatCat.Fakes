using System;
using System.Collections.Generic;

namespace OneOff.Models
{
    public class PadCleanRoom : PadFeature
    {
        public int NumberOfPeople { get; set; }

        public Guid CleanHash { get; set; }

        public List<Equipment> Equipments { get; set; }
    }
}
