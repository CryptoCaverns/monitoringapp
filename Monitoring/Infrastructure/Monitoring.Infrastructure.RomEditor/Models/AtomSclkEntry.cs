using System.Runtime.InteropServices;

namespace Monitoring.Infrastructure.RomEditor.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct AtomSclkEntry
    {
        public byte ucVddInd;
        public ushort usVddcOffset;
        public uint ulSclk;
        public ushort usEdcCurrent;
        public byte ucReliabilityTemperature;
        public byte ucCKSVOffsetAndDisable;
        public uint ulSclkOffset;
        // Polaris Only, remove for compatibility with Fiji
    };
}