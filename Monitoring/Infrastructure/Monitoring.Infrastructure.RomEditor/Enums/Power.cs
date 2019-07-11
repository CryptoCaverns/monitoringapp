using System.ComponentModel;

namespace Monitoring.Infrastructure.RomEditor.Enums
{
    public enum Power
    {
        [Description("Lower core clocks and core voltages also lower memory voltages and OC memory.")]
        PowerSaving,

        [Description("Stock core clocks and lower core voltages also lower memory voltages and OC memor.")]
        OcWithSlightUnderVolting
    }
}