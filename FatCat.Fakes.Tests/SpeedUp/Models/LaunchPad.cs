using System.Collections.Generic;

namespace FatCat.Fakes.Tests.SpeedUp.Models
{
public class LaunchPad
{
	public List<Mission> CompletedMissions { get; set; }

	public List<PadFeature> Features { get; set; }

	public int Latitude { get; set; }

	public int Longitude { get; set; }

	public List<Mission> MissionsInProgress { get; set; }

	public string Name { get; set; }
}
}