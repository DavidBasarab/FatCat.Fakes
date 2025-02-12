using System.Collections.Generic;

namespace OneOff.Models;

public class Rocket : SpaceCraft
{
    public List<Payload> Payloads { get; set; }

    public float PayloadSize { get; set; }

    public float Thrust { get; set; }
}
