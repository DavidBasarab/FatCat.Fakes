using System;

namespace FatCat.Fakes.Tests.SpeedUp.Models
{
public class WaterTower : PadFeature
{
	public TimeSpan TimeToFile { get; set; }

	public float TotalLiters { get; set; }
}
}