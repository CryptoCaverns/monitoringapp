using System.Runtime.InteropServices;

namespace Monitoring.Infrastructure.RomEditor.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct AtomVoltageTable
    {
        public byte ucRevId;
        public byte ucNumEntries;
        // public ATOM_VOLTAGE_ENTRY entries[ucNumEntries];
    };
}