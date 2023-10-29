namespace OneOff.Models
{
public class Chair : Equipment
{
	public bool HasArms { get; set; }

	public bool HasWheels { get; set; }

	public int Height { get; set; }

	public int NumberOfWheels { get; set; }
}
}