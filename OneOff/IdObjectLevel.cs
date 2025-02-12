using MongoDB.Bson;

namespace OneOff;

public class IdObjectLevel : BottomLevel
{
    public ObjectId Id { get; set; }
}
