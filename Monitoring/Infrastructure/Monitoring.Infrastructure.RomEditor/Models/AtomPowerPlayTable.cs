using System.Runtime.InteropServices;

namespace Monitoring.Infrastructure.RomEditor.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    unsafe struct AtomPowerPlayTable
    {
        public AtomCommonTableHeader sHeader;
        public byte ucTableRevision;
        public ushort usTableSize;
        public uint ulGoldenPPID;
        public uint ulGoldenRevision;
        public ushort usFormatID;
        public ushort usVoltageTime;
        public uint ulPlatformCaps;
        public uint ulMaxODEngineClock;
        public uint ulMaxODMemoryClock;
        public ushort usPowerControlLimit;
        public ushort usUlvVoltageOffset;
        public ushort usStateArrayOffset;
        public ushort usFanTableOffset;
        public ushort usThermalControllerOffset;
        public ushort usReserv;
        public ushort usMclkDependencyTableOffset;
        public ushort usSclkDependencyTableOffset;
        public ushort usVddcLookupTableOffset;
        public ushort usVddgfxLookupTableOffset;
        public ushort usMMDependencyTableOffset;
        public ushort usVCEStateTableOffset;
        public ushort usPPMTableOffset;
        public ushort usPowerTuneTableOffset;
        public ushort usHardLimitTableOffset;
        public ushort usPCIETableOffset;
        public ushort usGPIOTableOffset;
        public fixed ushort usReserved[6];
    };
}