using System;

namespace Monitoring.Infrastructure.MongoDB.Documents
{
    public class MinerLogDocument
    {
        public string Id { get; set; }

        public string MiningUnitId { get; set; }

        public int HashRate { get; set; }

        public double AvgTemp { get; set; }

        public DateTime TimeStamp { get; set; }

        public bool HasMemoryError { get; set; }
    }
}