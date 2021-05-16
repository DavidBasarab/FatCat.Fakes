using System;

namespace OneOff.Models
{
	public class WaterTower : PadFeature
	{
		public float TotalLiters { get; set; }

		public TimeSpan TimeToFile { get; set; }
	}
}