using System.Runtime.InteropServices;

namespace Monitoring.Infrastructure.RomEditor.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct AtomCommonTableHeader
    {
        public short usStructureSize;
        public byte ucTableFormatRevision;
        public byte ucTableContentRevision;
    }
}