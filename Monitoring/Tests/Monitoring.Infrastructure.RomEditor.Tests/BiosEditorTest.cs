using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Monitoring.Infrastructure.RomEditor.Dto;
using Monitoring.Infrastructure.RomEditor.Enums;
using Xunit;

namespace Monitoring.Infrastructure.RomEditor.Tests
{
    public class BiosEditorTest
    {
        private const string DefaultRomDirectory = "382";

        [Fact]
        public void ReOpenFile()
        {
            Assert.True(File.Exists(InputFile), "Input file doesn't exist.");

            var input = InputFileBytes;
            var editor = new BiosEditor();
            editor.Open(new MemoryStream(input));
            var output = editor.Save().ToArray();
            Assert.True(input.SequenceEqual(output), "Save functionality works incorrect.");
        }

        [Fact]
        public void SysLabelUpdate()
        {
            Assert.True(File.Exists(InputFile), "Input file doesn't exist.");
            Assert.True(File.Exists(FileName("check-name")), "Input file doesn't exist.");

            var input = InputFileBytes;
            var editor = new BiosEditor();
            editor.Open(new MemoryStream(input));
            editor.BiosBootUpMessage = "f610dd53-4c2c-4719-994f-fef8867423a0";
            var output = editor.Save().ToArray();

            Assert.True(output.SequenceEqual(FileBytes("check-name")), "Update sys label works incorrect.");
        }

        [Fact]
        public void SamsungWindows()
        {
            Assert.True(File.Exists(InputFile), "Input file doesn't exist.");
            Assert.True(File.Exists(FileName("s_w")), "Input file doesn't exist.");

            var input = InputFileBytes;
            var editor = new BiosEditor();
            var options = new PathTimingOption
            {
                Os = OS.Windows
            };
            editor.Open(new MemoryStream(input));
            editor.PathTiming(options);
            var output = editor.Save().ToArray();

            Assert.True(output.SequenceEqual(FileBytes("s_w")), "Path timing works incorrect.");
        }

        [Fact]
        public void SamsungLinux()
        {
            Assert.True(File.Exists(InputFile), "Input file doesn't exist.");
            Assert.True(File.Exists(FileName("s_l")), "Input file doesn't exist.");

            var input = InputFileBytes;
            var editor = new BiosEditor();
            var options = new PathTimingOption
            {
                Os = OS.Linux
            };
            editor.Open(new MemoryStream(input));
            editor.PathTiming(options);
            var output = editor.Save().ToArray();

            Assert.True(output.SequenceEqual(FileBytes("s_l")), "Path timing works incorrect.");
        }

        [Fact]
        public void SamsungWindowsPowerOc()
        {
            Assert.True(File.Exists(InputFile), "Input file doesn't exist.");
            Assert.True(File.Exists(FileName("s_w_oc")), "Input file doesn't exist.");

            var input = InputFileBytes;
            var editor = new BiosEditor();
            var options = new PathTimingOption
            {
                Os = OS.Windows,
                Power = Power.OcWithSlightUnderVolting
            };
            editor.Open(new MemoryStream(input));
            editor.PathTiming(options);
            var output = editor.Save().ToArray();

            Assert.True(output.SequenceEqual(FileBytes("s_w_oc")), "Path timing works incorrect.");
        }

        [Fact]
        public void SamsungLinuxPowerOc()
        {
            Assert.True(File.Exists(InputFile), "Input file doesn't exist.");
            Assert.True(File.Exists(FileName("s_l_oc")), "Input file doesn't exist.");

            var input = InputFileBytes;
            var editor = new BiosEditor();
            var options = new PathTimingOption
            {
                Os = OS.Linux,
                Power = Power.OcWithSlightUnderVolting
            };
            editor.Open(new MemoryStream(input));
            editor.PathTiming(options);
            var output = editor.Save().ToArray();

            Assert.True(output.SequenceEqual(FileBytes("s_l_oc")), "Path timing works incorrect.");
        }

        [Fact]
        public void SamsungWindowsPowerSavingEth()
        {
            Assert.True(File.Exists(InputFile), "Input file doesn't exist.");
            Assert.True(File.Exists(FileName("s_w_ps_eth")), "Input file doesn't exist.");

            var input = InputFileBytes;
            var editor = new BiosEditor();
            var options = new PathTimingOption
            {
                Os = OS.Windows,
                Power = Power.PowerSaving,
                Algorithm = Algorithm.ETH
            };
            editor.Open(new MemoryStream(input));
            editor.PathTiming(options);
            var output = editor.Save().ToArray();

            Assert.True(output.SequenceEqual(FileBytes("s_w_ps_eth")), "Path timing works incorrect.");
        }

        [Fact]
        public void SamsungWindowsPowerSavingXmr()
        {
            Assert.True(File.Exists(InputFile), "Input file doesn't exist.");
            Assert.True(File.Exists(FileName("s_w_ps_xmr")), "Input file doesn't exist.");

            var input = InputFileBytes;
            var editor = new BiosEditor();
            var options = new PathTimingOption
            {
                Os = OS.Windows,
                Power = Power.PowerSaving,
                Algorithm = Algorithm.XMR
            };
            editor.Open(new MemoryStream(input));
            editor.PathTiming(options);
            var output = editor.Save().ToArray();

            Assert.True(output.SequenceEqual(FileBytes("s_w_ps_xmr")), "Path timing works incorrect.");
        }

        [Fact]
        public void SamsungLinuxU311()
        {
            Assert.True(File.Exists(InputFile), "Input file doesn't exist.");
            Assert.True(File.Exists(FileName("s_l_u311")), "Input file doesn't exist.");

            var input = InputFileBytes;
            var editor = new BiosEditor();
            var options = new PathTimingOption
            {
                Os = OS.Linux,
                IsUberMix311 = true
            };
            editor.Open(new MemoryStream(input));
            editor.PathTiming(options);
            var output = editor.Save().ToArray();

            Assert.True(output.SequenceEqual(FileBytes("s_l_u311")), "Path timing works incorrect.");
        }

        [Fact]
        public void SamsungLinuxU32()
        {
            Assert.True(File.Exists(InputFile), "Input file doesn't exist.");
            Assert.True(File.Exists(FileName("s_l_u32")), "Input file doesn't exist.");

            var input = InputFileBytes;
            var editor = new BiosEditor();
            var options = new PathTimingOption
            {
                Os = OS.Linux,
                IsUberMix311 = false,
                IsUberMix32 = true
            };
            editor.Open(new MemoryStream(input));
            editor.PathTiming(options);
            var output = editor.Save().ToArray();

            Assert.True(output.SequenceEqual(FileBytes("s_l_u32")), "Path timing works incorrect.");
        }

        [Fact]
        public void SamsungLinuxOther()
        {
            Assert.True(File.Exists(InputFile), "Input file doesn't exist.");
            Assert.True(File.Exists(FileName("s_l_other")), "Input file doesn't exist.");

            var input = InputFileBytes;
            var editor = new BiosEditor();
            var options = new PathTimingOption
            {
                Os = OS.Linux,
                IsUberMix311 = false,
                IsUberMix32 = false
            };
            editor.Open(new MemoryStream(input));
            editor.PathTiming(options);
            var output = editor.Save().ToArray();

            Assert.True(output.SequenceEqual(FileBytes("s_l_other")), "Path timing works incorrect.");
        }

        private string InputFile => $"RomFiles/{DefaultRomDirectory}/input.rom";
        private byte[] InputFileBytes
        {
            get
            {
                using (var fs = File.Open(InputFile, FileMode.Open))
                using (var br = new BinaryReader(fs))
                {
                    return br.ReadBytes((int)fs.Length);
                }
            }
        }

        private string FileName(string name) => $"RomFiles/{DefaultRomDirectory}/{name}.rom";
        private byte[] FileBytes(string name)
        {
            using (var fs = File.Open(FileName(name), FileMode.Open))
            using (var br = new BinaryReader(fs))
            {
                return br.ReadBytes((int)fs.Length);
            }
        }
    }
}
