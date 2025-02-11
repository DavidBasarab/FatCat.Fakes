namespace FatCat.Fakes.Tests.SpeedUp.Models
{
    public class Astronaut : Personnel
    {
        public bool InSpace { get; set; }

        public float KilometersTraveled { get; set; }

        public List<Mission> Missions { get; set; }

        public string Speciality { get; set; }

        public TimeSpan TimeInSpace { get; set; }

        public int TimesInSpace { get; set; }
    }
}
