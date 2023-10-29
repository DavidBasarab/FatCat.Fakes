namespace OneOff.Models
{
public class Monitors : Equipment
{
	public int NumberOfHdmis { get; set; }

	public bool On { get; set; }

	public Resolution Resolution { get; set; }
}
}