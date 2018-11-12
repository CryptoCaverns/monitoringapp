using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Monitoring.Infrastructure.MongoDB.Documents
{
    public class MongoEntity : IMongoEntity
    {
        [BsonId]
        [JsonIgnore]
        public ObjectId Id { get; set; }

        [BsonIgnore]
        public string StringId => Id.ToString();
    }
}