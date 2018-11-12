using MongoDB.Bson;

namespace Monitoring.Infrastructure.MongoDB.Documents
{
    public interface IMongoEntity
    {
        ObjectId Id { get; set; }
    }
}