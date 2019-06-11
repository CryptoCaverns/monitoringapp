using System.Runtime.InteropServices;

namespace Monitoring.Infrastructure.RomEditor.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct AtomVoltageEntry
    {
        public ushort usVdd;
        public ushort usCACLow;
        public ushort usCACMid;
        public ushort usCACHigh;
    };
}