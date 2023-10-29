namespace FatCat.Fakes.Tests.SpeedUp.Models
{
    public class Resolution
    {
        public int Height { get; set; }

        public int TotalPixels => Width * Height;

        public int Width { get; set; }
    }
}
