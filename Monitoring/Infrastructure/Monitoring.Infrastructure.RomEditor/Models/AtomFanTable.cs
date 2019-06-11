using System.Runtime.InteropServices;

namespace Monitoring.Infrastructure.RomEditor.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct AtomFanTable
    {
        public byte ucRevId;
        public byte ucTHyst;
        public ushort usTMin;
        public ushort usTMed;
        public ushort usTHigh;
        public ushort usPWMMin;
        public ushort usPWMMed;
        public ushort usPWMHigh;
        public ushort usTMax;
        public byte ucFanControlMode;
        public ushort usFanPWMMax;
        public ushort usFanOutputSensitivity;
        public ushort usFanRPMMax;
        public uint ulMinFanSCLKAcousticLimit;
        public byte ucTargetTemperature;
        public byte ucMinimumPWMLimit;
        public ushort usFanGainEdge;
        public ushort usFanGainHotspot;
        public ushort usFanGainLiquid;
        public ushort usFanGainVrVddc;
        public ushort usFanGainVrMvdd;
        public ushort usFanGainPlx;
        public ushort usFanGainHbm;
        public ushort usReserved;
    };
}