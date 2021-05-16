using System;

namespace FatCat.Fakes.Tests.SpeedUp.Models
{
	public class Tower : PadFeature
	{
		public float Height { get; set; }

		public float TotalMass { get; set; }

		public float TotalForceOfLaunch { get; set; }

		public DateTime LastPainted { get; set; }
	}
}