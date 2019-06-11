using System.Runtime.InteropServices;

namespace Monitoring.Infrastructure.RomEditor.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct AtomRomHeader
    {
        public AtomCommonTableHeader sHeader;
        //public UInt32 uaFirmWareSignature;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] uaFirmWareSignature;
        public ushort usBiosRuntimeSegmentAddress;
        public ushort usProtectedModeInfoOffset;
        public ushort usConfigFilenameOffset;
        public ushort usCRC_BlockOffset;
        public ushort usBIOS_BootUpMessageOffset;
        public ushort usInt10Offset;
        public ushort usPciBusDevInitCode;
        public ushort usIoBaseAddress;
        public ushort usSubsystemVendorID;
        public ushort usSubsystemID;
        public ushort usPCI_InfoOffset;
        public ushort usMasterCommandTableOffset;
        public ushort usMasterDataTableOffset;
        public byte ucExtendedFunctionCode;
        public byte ucReserved;
        public uint ulPSPDirTableOffset;
        public ushort usVendorID;
        public ushort usDeviceID;
    }
}