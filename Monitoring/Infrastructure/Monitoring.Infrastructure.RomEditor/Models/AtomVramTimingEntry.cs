using System.Runtime.InteropServices;

namespace Monitoring.Infrastructure.RomEditor.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct AtomVramTimingEntry
    {
        public uint ulClkRange;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x30)]
        public byte[] ucLatency;
    };
}