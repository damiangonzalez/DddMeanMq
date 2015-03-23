using MongoDB.Bson;

namespace Domain
{
    public abstract class AggregateRootBase
    {
        public ObjectId Id { get; set; }

        public string Notes { get; set; }
    }
}
