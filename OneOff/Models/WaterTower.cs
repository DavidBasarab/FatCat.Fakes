using System;

namespace FatCat.Fakes.Tests.SpeedUp.Models
{
	public class WaterTower : PadFeature
	{
		public float TotalLiters { get; set; }

		public TimeSpan TimeToFile { get; set; }
	}
}