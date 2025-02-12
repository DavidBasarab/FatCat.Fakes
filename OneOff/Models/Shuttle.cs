namespace OneOff.Models;

public class Shuttle : SpaceCraft
{
    public bool BayDoorsOpen { get; set; }

    public int CrewSize { get; set; }

    public float FuelRemaining { get; set; }
}
