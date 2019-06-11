using System.Runtime.InteropServices;

namespace Monitoring.Infrastructure.RomEditor.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct AtomMclkEntry
    {
        public byte ucVddcInd;
        public ushort usVddci;
        public ushort usVddgfxOffset;
        public ushort usMvdd;
        public uint ulMclk;
        public ushort usReserved;
    };
}