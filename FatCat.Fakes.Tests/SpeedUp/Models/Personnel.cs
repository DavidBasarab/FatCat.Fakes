using System;

namespace FatCat.Fakes.Tests.SpeedUp.Models
{
	public class Personnel
	{
		public DateTime Created { get; set; }

		public PersonnelStatus PersonnelStatus { get; set; }

		public DateTime StartDate { get; set; }

		public string Title { get; set; }

		public string Username { get; set; }
	}
}