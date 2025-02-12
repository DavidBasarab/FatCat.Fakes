using System;
using System.Collections.Generic;

namespace OneOff.Models
{
    public class Nasa
    {
        public List<Administrator> Administrators { get; set; }

        public List<Personnel> AllPersonnel { get; set; }

        public List<SpaceLaunchCenter> Centers { get; set; }

        public DateTime Founded { get; set; }

        public List<Mission> HistoricMissions { get; set; }

        public string LatestHeadline { get; set; }

        public List<Mission> Missions { get; set; }

        public List<SpaceCraft> SpaceCraft { get; set; }
    }
}
