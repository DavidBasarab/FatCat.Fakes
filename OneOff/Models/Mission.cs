using System;
using System.Collections.Generic;

namespace OneOff.Models
{
    public class Mission
    {
        public TimeSpan ActiveTime { get; set; }

        public bool IsActive { get; set; }

        public DateTime LaunchDate { get; set; }

        public List<SpaceCraft> SpaceCrafts { get; set; }
    }
}
