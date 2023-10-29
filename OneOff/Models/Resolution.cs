namespace OneOff.Models
{
    public class Resolution
    {
        public int TotalPixels => Width * Height;

        public int Height { get; set; }

        public int Width { get; set; }
    }
}
