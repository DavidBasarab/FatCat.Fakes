namespace OneOff.Models
{
    public class Monitors : Equipment
    {
        public Resolution Resolution { get; set; }

        public bool On { get; set; }

        public int NumberOfHdmis { get; set; }
    }
}
