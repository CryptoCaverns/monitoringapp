using System.Runtime.InteropServices;

namespace Monitoring.Infrastructure.RomEditor.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct AtomPowerTuneTable
    {
        public byte ucRevId;
        public ushort usTDP;
        public ushort usConfigurableTDP;
        public ushort usTDC;
        public ushort usBatteryPowerLimit;
        public ushort usSmallPowerLimit;
        public ushort usLowCACLeakage;
        public ushort usHighCACLeakage;
        public ushort usMaximumPowerDeliveryLimit;
        public ushort usTjMax;
        public ushort usPowerTuneDataSetID;
        public ushort usEDCLimit;
        public ushort usSoftwareShutdownTemp;
        public ushort usClockStretchAmount;
        public ushort usTemperatureLimitHotspot;
        public ushort usTemperatureLimitLiquid1;
        public ushort usTemperatureLimitLiquid2;
        public ushort usTemperatureLimitVrVddc;
        public ushort usTemperatureLimitVrMvdd;
        public ushort usTemperatureLimitPlx;
        public byte ucLiquid1_I2C_address;
        public byte ucLiquid2_I2C_address;
        public byte ucLiquid_I2C_Line;
        public byte ucVr_I2C_address;
        public byte ucVr_I2C_Line;
        public byte ucPlx_I2C_address;
        public byte ucPlx_I2C_Line;
        public ushort usReserved;
    };
}