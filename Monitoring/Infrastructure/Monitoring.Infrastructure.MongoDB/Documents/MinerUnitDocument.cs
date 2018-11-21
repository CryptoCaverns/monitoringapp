using System;

namespace Monitoring.Infrastructure.MongoDB.Documents
{
    public class MinerUnitDocument : MongoEntity
    {
        public GPU GPU { get; set; }

        public Motherboard MotherBoard { get; set; }

        public Tuning TuningSettings { get; set; }

        public BIOS BIOSSettings { get; set; }

        public Coin Coin { get; set; }

        public DateTime CreatedTimestamp { get; set; }
    }

    public class GPU
    {
        public string SysLabel { get; set; }

        public string Name { get; set; }

        public bool HasRiser { get; set; }
    }

    public class Motherboard
    {
        public string SysLabel { get; set; }

        public string MacAddress { get; set; }

        public string IPAddress { get; set; }
    }

    public class BIOS
    {
        public string MemoryStrapping { get; set; }
        public string SettingsHash { get; set; }
    }

    public class Tuning
    {
        public int CPUClockSpeed { get; set; }

        public int MemoryClockSpeed { get; set; }

        public double CPUVoltage { get; set; }

        public double VRAMVoltage { get; set; }
    }

    public class Coin
    {
        public string Name { get; set; }

        public string Algorithm { get; set; }
    }
}