using System;

namespace OneOff;

public class TestingMongoObject : IdObjectLevel
{
    public string Name { get; set; }

    public int Number { get; set; }

    public DateTime SomeDate { get; set; }
}
