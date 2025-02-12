using System;

namespace OneOff.Models;

public abstract class SpaceCraft : Equipment
{
    public bool InSpace { get; set; }

    public DateTime LaunchDate { get; set; }

    public SpaceCraftStatus SpaceCraftStatus { get; set; }
}
