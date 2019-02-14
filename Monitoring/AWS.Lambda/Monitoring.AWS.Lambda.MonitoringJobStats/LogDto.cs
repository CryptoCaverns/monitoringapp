using Monitoring.Infrastructure.MongoDB.Documents;

namespace Monitoring.AWS.Lambda.MonitoringJobStats
{
    public class LogDto
    {
        public MinerLogDocument GPU { get; set; }
    }
}