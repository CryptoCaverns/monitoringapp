namespace Monitoring.Infrastructure.RomEditor.Dto
{
    public class PowerTune
    {
        /// <summary>
        /// TDP (W)
        /// </summary>
        public ushort TDP { get; set; }
        /// <summary>
        /// TDC (A)
        /// </summary>
        public ushort TDC { get; set; }
        /// <summary>
        /// Max Power Limit (W)
        /// </summary>
        public ushort MaxPowerLimit { get; set; }
        /// <summary>
        /// Max Temp. (C)
        /// </summary>
        public ushort MaxTemp { get; set; }
        /// <summary>
        /// Shutdown Temp. (C)
        /// </summary>
        public ushort ShutdownTemp { get; set; }
        /// <summary>
        /// Hotspot Temp. (C)
        /// </summary>
        public ushort HotspotTemp { get; set; }

        public ushort ClockStretchAmount { get; set; }
    }
}