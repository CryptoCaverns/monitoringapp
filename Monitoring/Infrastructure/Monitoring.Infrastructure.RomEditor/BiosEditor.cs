using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Monitoring.Infrastructure.RomEditor.Dto;
using Monitoring.Infrastructure.RomEditor.Helpers;
using Monitoring.Infrastructure.RomEditor.Models;

namespace Monitoring.Infrastructure.RomEditor
{
    public class BiosEditor
    {
        public string CheckSum { get; set; }
        public string BiosBootUpMessage
        {
            get
            {
                if (_biosBootUpMessage.Length > 36)
                    return _biosBootUpMessage.Substring(0, 36);

                return _biosBootUpMessage;
            }
            set => _biosBootUpMessage = value.Trim();
        }

        public IEnumerable<Speed> ClockSpeed
        {
            get
            { 
                for (var i = 0; i < _atomSclkTable.ucNumEntries; i++)
                {
                    yield return new Speed
                    {
                        FreqMHz = _atomSclkEntries[i].ulSclk / 100,
                        Voltage = _atomVddcEntries[_atomSclkEntries[i].ucVddInd].usVdd
                    };
                }
            }
        }

        public IEnumerable<Speed> MemorySpeed
        {
            get
            {
                for (var i = 0; i < _atomMclkTable.ucNumEntries; i++)
                {
                    yield return new Speed
                    {
                        FreqMHz = _atomMclkEntries[i].ulMclk / 100,
                        Voltage = _atomMclkEntries[i].usMvdd
                    };
                }
            }
        }

        public PowerTune PowerTune =>
            new PowerTune
            {
                TDP = _atomPowerTuneTable.usTDP,
                TDC = _atomPowerTuneTable.usTDC,
                MaxPowerLimit = _atomPowerTuneTable.usMaximumPowerDeliveryLimit,
                MaxTemp = _atomPowerTuneTable.usTjMax,
                ShutdownTemp = _atomPowerTuneTable.usSoftwareShutdownTemp,
                HotspotTemp = _atomPowerTuneTable.usTemperatureLimitHotspot,
                ClockStretchAmount = _atomPowerTuneTable.usClockStretchAmount
            };

        private const int MaxVramEntries = 48; // e.g. MSI-Armor-RX-580-4GB has 36 entries
        public readonly Dictionary<string, string> Rc = new Dictionary<string, string>();

        private byte[] _buffer;
        private string _deviceId = "";

        private int _atomRomChecksumOffset = 0x21;
        private int _atomRomHeaderPtr = 0x48;
        private int _atomRomHeaderOffset;
        private AtomRomHeader _atomRomHeader;
        private AtomDataTables _atomDataTable;

        private int _atomPowerPlayOffset;
        private AtomPowerPlayTable _atomPowerPlayTable;

        private int _atomPowerTuneOffset;
        private AtomPowerTuneTable _atomPowerTuneTable;

        private int _atomFanOffset;
        private AtomFanTable _atomFanTable;

        private int _atomMclkTableOffset;
        private AtomMclkTable _atomMclkTable;
        private AtomMclkEntry[] _atomMclkEntries;

        private int _atomSclkTableOffset;
        private AtomSclkTable _atomSclkTable;
        private AtomSclkEntry[] _atomSclkEntries;

        private int _atomVddcTableOffset;
        private AtomVoltageTable _atomVddcTable;
        private AtomVoltageEntry[] _atomVddcEntries;

        private int _atomVramInfoOffset;
        private AtomVramInfo _atomVramInfo;
        private AtomVramEntry[] _atomVramEntries;
        private AtomVramTimingEntry[] _atomVramTimingEntries;

        private int _atomVramTimingOffset;
        private string _biosBootUpMessage;

        public BiosEditor()
        {
            Rc.Add("MT51J256M3", "MICRON");
            Rc.Add("EDW4032BAB", "ELPIDA");
            Rc.Add("H5GC4H24AJ", "HYNIX_1");
            Rc.Add("H5GQ8H24MJ", "HYNIX_2");
            Rc.Add("H5GC8H24MJ", "HYNIX_2");
            Rc.Add("K4G80325FB", "SAMSUNG");
            Rc.Add("K4G41325FE", "SAMSUNG");
            Rc.Add("K4G41325FC", "SAMSUNG");
            Rc.Add("K4G41325FS", "SAMSUNG");
        }

        public void Open(Stream stream)
        {
            if (stream.Length != 524288 && stream.Length != 524288 / 2)
            {
                throw new Exception("This BIOS is non standard size.\nFlashing this BIOS may corrupt your graphics card.");
            }

            using (var br = new BinaryReader(stream))
            {
                _buffer = br.ReadBytes((int)stream.Length);

                _atomRomHeaderOffset = GetValueAtPosition(16, _atomRomHeaderPtr);
                _atomRomHeader = _buffer.Skip(_atomRomHeaderOffset).ToArray().FromBytes<AtomRomHeader>();
                _deviceId = _atomRomHeader.usDeviceID.ToString("X");
                FixCheckSum(false);

                var firmwareSignature = new string(_atomRomHeader.uaFirmWareSignature);
                if (!firmwareSignature.Equals("ATOM"))
                {
                    throw new Exception("WARNING! BIOS Signature is not valid. Only continue if you are 100% sure what you are doing!");
                }

                if (!Constants.Constants.SupportedDeviceId.Contains(_deviceId))
                {
                    throw new Exception("Unsupported DeviceID 0x" + _deviceId);
                }

                var sb = new StringBuilder();

                var ptr = _atomRomHeader.usBIOS_BootUpMessageOffset + 2;
                while (ptr != -1)
                {
                    var c = (char)_buffer[ptr];
                    if (c == '\0')
                    {
                        ptr = -1;
                    }
                    else if (c == '\n' || c == '\r')
                    {
                        ptr++;
                    }
                    else
                    {
                        sb.Append(c);
                        ptr++;
                    }
                }

                _biosBootUpMessage = sb.ToString().Trim();

                _atomDataTable = _buffer.Skip(_atomRomHeader.usMasterDataTableOffset).ToArray().FromBytes<AtomDataTables>();
                _atomPowerPlayOffset = _atomDataTable.PowerPlayInfo;
                _atomPowerPlayTable = _buffer.Skip(_atomPowerPlayOffset).ToArray().FromBytes<AtomPowerPlayTable>();

                _atomPowerTuneOffset = _atomDataTable.PowerPlayInfo + _atomPowerPlayTable.usPowerTuneTableOffset;
                _atomPowerTuneTable = _buffer.Skip(_atomPowerTuneOffset).ToArray().FromBytes<AtomPowerTuneTable>();

                _atomFanOffset = _atomDataTable.PowerPlayInfo + _atomPowerPlayTable.usFanTableOffset;
                _atomFanTable = _buffer.Skip(_atomFanOffset).ToArray().FromBytes<AtomFanTable>();

                _atomMclkTableOffset = _atomDataTable.PowerPlayInfo + _atomPowerPlayTable.usMclkDependencyTableOffset;
                _atomMclkTable = _buffer.Skip(_atomMclkTableOffset).ToArray().FromBytes<AtomMclkTable>();
                _atomMclkEntries = new AtomMclkEntry[_atomMclkTable.ucNumEntries];
                for (var i = 0; i < _atomMclkEntries.Length; i++)
                {
                    _atomMclkEntries[i] = _buffer.Skip(_atomMclkTableOffset + Marshal.SizeOf(typeof(AtomMclkTable)) + Marshal.SizeOf(typeof(AtomMclkEntry)) * i).ToArray().FromBytes<AtomMclkEntry>();
                }

                _atomSclkTableOffset = _atomDataTable.PowerPlayInfo + _atomPowerPlayTable.usSclkDependencyTableOffset;
                _atomSclkTable = _buffer.Skip(_atomSclkTableOffset).ToArray().FromBytes<AtomSclkTable>();
                _atomSclkEntries = new AtomSclkEntry[_atomSclkTable.ucNumEntries];
                for (var i = 0; i < _atomSclkEntries.Length; i++)
                {
                    _atomSclkEntries[i] = _buffer.Skip(_atomSclkTableOffset + Marshal.SizeOf(typeof(AtomSclkTable)) + Marshal.SizeOf(typeof(AtomSclkEntry)) * i).ToArray().FromBytes<AtomSclkEntry>();
                }

                _atomVddcTableOffset = _atomDataTable.PowerPlayInfo + _atomPowerPlayTable.usVddcLookupTableOffset;
                _atomVddcTable = _buffer.Skip(_atomVddcTableOffset).ToArray().FromBytes<AtomVoltageTable>();
                _atomVddcEntries = new AtomVoltageEntry[_atomVddcTable.ucNumEntries];
                for (var i = 0; i < _atomVddcTable.ucNumEntries; i++)
                {
                    _atomVddcEntries[i] = _buffer.Skip(_atomVddcTableOffset + Marshal.SizeOf(typeof(AtomVoltageTable)) + Marshal.SizeOf(typeof(AtomVoltageEntry)) * i).ToArray().FromBytes<AtomVoltageEntry>();
                }

                _atomVramInfoOffset = _atomDataTable.VRAM_Info;
                _atomVramInfo = _buffer.Skip(_atomVramInfoOffset).ToArray().FromBytes<AtomVramInfo>();
                _atomVramEntries = new AtomVramEntry[_atomVramInfo.ucNumOfVRAMModule];
                var atomVramEntryOffset = _atomVramInfoOffset + Marshal.SizeOf(typeof(AtomVramInfo));
                for (var i = 0; i < _atomVramInfo.ucNumOfVRAMModule; i++)
                {
                    _atomVramEntries[i] = _buffer.Skip(atomVramEntryOffset).ToArray().FromBytes<AtomVramEntry>();
                    atomVramEntryOffset += _atomVramEntries[i].usModuleSize;
                }

                _atomVramTimingOffset = _atomVramInfoOffset + _atomVramInfo.usMemClkPatchTblOffset + 0x2E;
                _atomVramTimingEntries = new AtomVramTimingEntry[MaxVramEntries];
                for (var i = 0; i < MaxVramEntries; i++)
                {
                    _atomVramTimingEntries[i] = _buffer.Skip(_atomVramTimingOffset + Marshal.SizeOf(typeof(AtomVramTimingEntry)) * i).ToArray().FromBytes<AtomVramTimingEntry>();

                    // atom_vram_timing_entries have an undetermined length
                    // attempt to determine the last entry in the array
                    if (_atomVramTimingEntries[i].ulClkRange == 0)
                    {
                        Array.Resize(ref _atomVramTimingEntries, i);
                        break;
                    }
                }

                stream.Close();
            }
        }

        public MemoryStream Save()
        {
            var stream = new MemoryStream();
            var bw = new BinaryWriter(stream);
            
            _buffer.SetBytesAtPosition(_atomRomHeaderOffset, _atomRomHeader.GetBytes());
            _buffer.SetBytesAtPosition(_atomPowerPlayOffset, _atomPowerPlayTable.GetBytes());
            _buffer.SetBytesAtPosition(_atomPowerTuneOffset, _atomPowerTuneTable.GetBytes());
            _buffer.SetBytesAtPosition(_atomFanOffset, _atomFanTable.GetBytes());

            for (var i = 0; i < _atomMclkTable.ucNumEntries; i++)
            {
                _buffer.SetBytesAtPosition(_atomMclkTableOffset + Marshal.SizeOf(typeof(AtomMclkTable)) + Marshal.SizeOf(typeof(AtomMclkEntry)) * i, _atomMclkEntries[i].GetBytes());
            }

            for (var i = 0; i < _atomSclkTable.ucNumEntries; i++)
            {
                _buffer.SetBytesAtPosition(_atomSclkTableOffset + Marshal.SizeOf(typeof(AtomSclkTable)) + Marshal.SizeOf(typeof(AtomSclkEntry)) * i, _atomSclkEntries[i].GetBytes());
            }

            for (var i = 0; i < _atomVddcTable.ucNumEntries; i++)
            {
                _buffer.SetBytesAtPosition(_atomVddcTableOffset + Marshal.SizeOf(typeof(AtomVoltageTable)) + Marshal.SizeOf(typeof(AtomVoltageEntry)) * i, _atomVddcEntries[i].GetBytes());
            }

            var atomVramEntryOffset = _atomVramInfoOffset + Marshal.SizeOf(typeof(AtomVramInfo));
            for (var i = 0; i < _atomVramInfo.ucNumOfVRAMModule; i++)
            {
                _buffer.SetBytesAtPosition(atomVramEntryOffset, _atomVramEntries[i].GetBytes());
                atomVramEntryOffset += _atomVramEntries[i].usModuleSize;
            }

            _atomVramTimingOffset = _atomVramInfoOffset + _atomVramInfo.usMemClkPatchTblOffset + 0x2E;
            for (var i = 0; i < _atomVramTimingEntries.Length; i++)
            {
                _buffer.SetBytesAtPosition(_atomVramTimingOffset + Marshal.SizeOf(typeof(AtomVramTimingEntry)) * i, _atomVramTimingEntries[i].GetBytes());
            }

            _buffer.SetBytesAtPosition(_atomRomHeader.usBIOS_BootUpMessageOffset + 2, Encoding.ASCII.GetBytes(_biosBootUpMessage));
            FixCheckSum(true);
            bw.Write(_buffer);

            return stream;
        }

        #region private

        public int GetValueAtPosition(int bits, int position, bool isFrequency = false)
        {
            if (position <= _buffer.Length - 4)
            {
                var value = 0;
                switch (bits)
                {
                    case 8:
                    default:
                        value = _buffer[position];
                        break;
                    case 16:
                        value = (_buffer[position + 1] << 8) | _buffer[position];
                        break;
                    case 24:
                        value = (_buffer[position + 2] << 16) | (_buffer[position + 1] << 8) | _buffer[position];
                        break;
                    case 32:
                        value = (_buffer[position + 3] << 24) | (_buffer[position + 2] << 16) | (_buffer[position + 1] << 8) | _buffer[position];
                        break;
                }
                if (isFrequency)
                    return value / 100;
                return value;
            }
            return -1;
        }
        private void FixCheckSum(bool save)
        {
            var checksum = _buffer[_atomRomChecksumOffset];
            var size = _buffer[0x02] * 512;
            byte offset = 0;

            for (var i = 0; i < size; i++)
            {
                offset += _buffer[i];
            }
            if (checksum == (_buffer[_atomRomChecksumOffset] - offset))
            {

            }
            else if (!save)
            {
                throw new Exception("Invalid checksum - Save to fix!");
            }
            if (save)
            {
                _buffer[_atomRomChecksumOffset] -= offset;
            }
            CheckSum = "0x" + _buffer[_atomRomChecksumOffset].ToString("X");
        }

        #endregion
    }
}