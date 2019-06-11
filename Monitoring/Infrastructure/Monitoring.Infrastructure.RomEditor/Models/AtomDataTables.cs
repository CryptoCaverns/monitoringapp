using System.Runtime.InteropServices;

namespace Monitoring.Infrastructure.RomEditor.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct AtomDataTables
    {
        public AtomCommonTableHeader sHeader;
        public ushort UtilityPipeLine;
        public ushort MultimediaCapabilityInfo;
        public ushort MultimediaConfigInfo;
        public ushort StandardVESA_Timing;
        public ushort FirmwareInfo;
        public ushort PaletteData;
        public ushort LCD_Info;
        public ushort DIGTransmitterInfo;
        public ushort SMU_Info;
        public ushort SupportedDevicesInfo;
        public ushort GPIO_I2C_Info;
        public ushort VRAM_UsageByFirmware;
        public ushort GPIO_Pin_LUT;
        public ushort VESA_ToInternalModeLUT;
        public ushort GFX_Info;
        public ushort PowerPlayInfo;
        public ushort GPUVirtualizationInfo;
        public ushort SaveRestoreInfo;
        public ushort PPLL_SS_Info;
        public ushort OemInfo;
        public ushort XTMDS_Info;
        public ushort MclkSS_Info;
        public ushort Object_Header;
        public ushort IndirectIOAccess;
        public ushort MC_InitParameter;
        public ushort ASIC_VDDC_Info;
        public ushort ASIC_InternalSS_Info;
        public ushort TV_VideoMode;
        public ushort VRAM_Info;
        public ushort MemoryTrainingInfo;
        public ushort IntegratedSystemInfo;
        public ushort ASIC_ProfilingInfo;
        public ushort VoltageObjectInfo;
        public ushort PowerSourceInfo;
        public ushort ServiceInfo;
    };
}