namespace FatCat.Fakes.Tests.SpeedUp.Models
{
    public class Mission
    {
        public TimeSpan ActiveTime { get; set; }

        public bool IsActive { get; set; }

        public DateTime LaunchDate { get; set; }

        public List<SpaceCraft> SpaceCrafts { get; set; }
    }
}
