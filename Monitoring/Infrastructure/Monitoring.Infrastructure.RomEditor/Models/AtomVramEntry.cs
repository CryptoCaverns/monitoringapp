using System.Runtime.InteropServices;

namespace Monitoring.Infrastructure.RomEditor.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct AtomVramEntry
    {
        public uint ulChannelMapCfg;
        public ushort usModuleSize;
        public ushort usMcRamCfg;
        public ushort usEnableChannels;
        public byte ucExtMemoryID;
        public byte ucMemoryType;
        public byte ucChannelNum;
        public byte ucChannelWidth;
        public byte ucDensity;
        public byte ucBankCol;
        public byte ucMisc;
        public byte ucVREFI;
        public ushort usReserved;
        public ushort usMemorySize;
        public byte ucMcTunningSetId;
        public byte ucRowNum;
        public ushort usEMRS2Value;
        public ushort usEMRS3Value;
        public byte ucMemoryVenderID;
        public byte ucRefreshRateFactor;
        public byte ucFIFODepth;
        public byte ucCDR_Bandwidth;
        public uint ulChannelMapCfg1;
        public uint ulBankMapCfg;
        public uint ulReserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] strMemPNString;
    };
}