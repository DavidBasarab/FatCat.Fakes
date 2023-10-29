using System;

namespace OneOff.Models
{
public class Tower : PadFeature
{
	public float Height { get; set; }

	public DateTime LastPainted { get; set; }

	public float TotalForceOfLaunch { get; set; }

	public float TotalMass { get; set; }
}
}