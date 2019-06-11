using System.Runtime.InteropServices;

namespace Monitoring.Infrastructure.RomEditor.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct AtomSclkTable
    {
        public byte ucRevId;
        public byte ucNumEntries;
        // public ATOM_SCLK_ENTRY entries[ucNumEntries];
    };
}