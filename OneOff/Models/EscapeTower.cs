using System;

namespace OneOff.Models
{
    public class EscapeTower : PadFeature
    {
        public int Height { get; set; }

        public DateTime LastInspection { get; set; }

        public int Run { get; set; }
    }
}
