using System.Collections.Generic;

namespace OneOff.Models
{
	public class MissionController : Personnel
	{
		public Education Education { get; set; }

		public List<Mission> Missions { get; set; }

		public string ShiftName { get; set; }

		public string Speciality { get; set; }
	}
}