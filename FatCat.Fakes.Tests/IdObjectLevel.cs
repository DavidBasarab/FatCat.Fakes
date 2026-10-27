using MongoDB.Bson;

namespace FatCat.Fakes.Tests;

public class IdObjectLevel : BottomLevel
{
    public ObjectId Id { get; set; }
}
