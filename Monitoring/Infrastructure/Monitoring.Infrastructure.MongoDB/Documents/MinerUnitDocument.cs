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
    }

    public class GPU
    {
        public string SysLabel { get; set; }
        public string PCIESlotId { get; set; }
        public string MacAddress { get; set; }
        public bool HasRiser { get; set; }
    }

    public class Motherboard
    {
        public string SysLabel { get; set; }
        public string SerialNumber { get; set; }
    }

    public class BIOS
    {
        public string MemoryStrapping { get; set; }
        public string BiosVersion { get; set; }
        public string BiosVendor { get; set; }
        public string BiosHash { get; set; }
    }

    public class Tunning
    {
        public string CPUClockSpeed { get; set; }
        public string MemoryClockSpeed { get; set; }
        public string CPUVoltage { get; set; }
        public string VRAMVoltage { get; set; }
        public string TimeStamp { get; set; }
    }

    public class Coin
    {
        public string Name { get; set; }
        public string Algorithm { get; set; }
    }
}