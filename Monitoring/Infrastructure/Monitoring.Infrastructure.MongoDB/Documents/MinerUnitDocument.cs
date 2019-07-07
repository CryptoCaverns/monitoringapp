using System;
using System.Collections.Generic;
using Monitoring.Infrastructure.RomEditor.Dto;

namespace Monitoring.Infrastructure.MongoDB.Documents
{
    public class MinerUnitDocument : MongoEntity
    {
        public DateTime CreatedTimestamp { get; set; }
        public string SysLabel { get; set; }
        public List<Speed> MemorySpeed { get; set; }
        public List<Speed> ClockSpeed { get; set; }
        public PowerTune PowerTune { get; set; }
        public string PCISlotId { get; set; }
        public string RigId { get; set; }
    }

    
    public class MinerRigDocument : MongoEntity
    {
        public string IpAddress { get; set; }
        public string SysLabel { get; set; }
        public string SerialNumber { get; set; }
    }
        
    public class Coin
    {
        public string Name { get; set; }
        public string Algorithm { get; set; }
    }
}