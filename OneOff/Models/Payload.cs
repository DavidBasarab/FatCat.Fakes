using System;

namespace OneOff.Models;

public class Payload : Equipment
{
    public DateTime DesignDate { get; set; }

    public DateTime Launched { get; set; }

    public float Mass { get; set; }

    public Guid PayloadId { get; set; }
}
