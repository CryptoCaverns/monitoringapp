using System.Runtime.InteropServices;

namespace Monitoring.Infrastructure.RomEditor.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct AtomVramInfo
    {
        public AtomCommonTableHeader sHeader;
        public ushort usMemAdjustTblOffset;
        public ushort usMemClkPatchTblOffset;
        public ushort usMcAdjustPerTileTblOffset;
        public ushort usMcPhyInitTableOffset;
        public ushort usDramDataRemapTblOffset;
        public ushort usReserved1;
        public byte ucNumOfVRAMModule;
        public byte ucMemoryClkPatchTblVer;
        public byte ucVramModuleVer;
        public byte ucMcPhyTileNum;
        // public ATOM_VRAM_ENTRY aVramInfo[ucNumOfVRAMModule];
    }
}