using System;

namespace Monitoring.Infrastructure.MongoDB.Documents
{
    public class BiosDocument : MongoEntity
    {
        public string Name { get; set; }
        public int OriginalHash { get; set; }
        public int CurrentHash { get; set; }
        public DateTime Timestamp { get; set; }
    }
}