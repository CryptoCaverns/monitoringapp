using System.ComponentModel;
using Monitoring.Infrastructure.RomEditor.Enums;

namespace Monitoring.Infrastructure.RomEditor.Dto
{
    public class PathTimingOption
    {
        public Strapping Strapping { get; set; }
        public Algorithm Algorithm { get; set; }
        public Power Power { get; set; }
        public OS Os { get; set; }
        public bool IsRx560 { get; set; }

        [Description("Do you want faster Uber-mix 3.1.1?")]
        public bool IsUberMix311 { get; set; }
        [Description("Do you want Uber-mix 3.2?")]
        public bool IsUberMix32 { get; set; }
        [Description("Do you want aggressive timing? RX 580 32MH/s, RX 570 31MH/s, not every card can handle it")]
        public bool IsAggressiveTiming { get; set; }
        [Description("Do you want Universal timing? More stable on some cards not the best hashrate")]
        public bool IsUniversalTiming { get; set; }
        [Description("Do you want Elpida 33+ MH Timing?")]
        public bool IsElpida33 { get; set; }

        public PathTimingOption()
        {
            // default values
            Strapping = Strapping.Strap1500Plus;
            Algorithm = Algorithm.ETH;
            Power = Power.OcWithSlightUnderVolting;
            Os = OS.Linux;
            IsRx560 = false;

            IsUniversalTiming = true;
            IsAggressiveTiming = true;
            IsElpida33 = true;
            IsUberMix311 = true;
            IsUberMix32 = true;
        }
    }
}