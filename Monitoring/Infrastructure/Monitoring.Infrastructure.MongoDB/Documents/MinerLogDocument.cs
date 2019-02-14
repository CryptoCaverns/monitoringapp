using System;

namespace Monitoring.Infrastructure.MongoDB.Documents
{
    public class MinerLogDocument : MongoEntity
    {
        public string MiningUnitId { get; set; }
        public string BiosHash { get; set; }
        public string SysLabel { get; set; }
        public string HashRate { get; set; }
        public string AvgTemp { get; set; }
        public string TimeStamp { get; set; }
        public bool HasMemoryError { get; set; }
    }
}