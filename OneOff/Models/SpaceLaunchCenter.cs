using System;
using System.Collections.Generic;

namespace OneOff.Models
{
	public class SpaceLaunchCenter
	{
		public string Name { get; set; }

		public Guid Id { get; set; }

		public List<LaunchPad> LaunchPads { get; set; }

		public List<Mission> CompletedMissions { get; set; }
		
		public List<Mission> MissionsInProgress { get; set; }

		public List<SpaceCraft> LaunchList { get; set; }
	}
}