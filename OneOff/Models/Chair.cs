namespace OneOff.Models
{
	public class Chair : Equipment
	{
		public int Height { get; set; }

		public bool HasArms { get; set; }

		public bool HasWheels { get; set; }

		public int NumberOfWheels { get; set; }
		
	}
}