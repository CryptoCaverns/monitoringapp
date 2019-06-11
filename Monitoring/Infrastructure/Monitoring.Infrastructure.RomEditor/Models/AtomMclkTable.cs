using System.Runtime.InteropServices;

namespace Monitoring.Infrastructure.RomEditor.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct AtomMclkTable
    {
        public byte ucRevId;
        public byte ucNumEntries;
        // public ATOM_MCLK_ENTRY entries[ucNumEntries];
    };
}