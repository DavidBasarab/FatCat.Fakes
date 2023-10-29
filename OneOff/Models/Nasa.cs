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

    public abstract class SpaceCraft : Equipment
    {
        public bool InSpace { get; set; }

        public DateTime LaunchDate { get; set; }

        public SpaceCraftStatus SpaceCraftStatus { get; set; }
    }

    public class Rocket : SpaceCraft
    {
        public List<Payload> Payloads { get; set; }

        public float PayloadSize { get; set; }

        public float Thrust { get; set; }
    }

    public class Shuttle : SpaceCraft
    {
        public bool BayDoorsOpen { get; set; }

        public int CrewSize { get; set; }

        public float FuelRemaining { get; set; }
    }

    public class Payload : Equipment
    {
        public DateTime DesignDate { get; set; }

        public DateTime Launched { get; set; }

        public float Mass { get; set; }

        public Guid PayloadId { get; set; }
    }

    public enum SpaceCraftStatus
    {
        UnderConstruction,
        Cancelled,
        Destroyed,
        Complete,
        Mothballed,
        Lost,
    }
}
