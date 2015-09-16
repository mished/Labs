using System.Collections.Generic;
using System.Management;

namespace BiosInfo {

    public static class BiosInfo {
        
        public static IEnumerable<string> GetBiosInfo() {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS")) {
                var info = searcher.Get();

                foreach(var infoObj in info) {
                    var biosVer  = (string[])infoObj["BIOSVersion"];
                    var biosChrs = (ushort[])infoObj["BiosCharacteristics"];

                    yield return
                        "Bios Version:\n"      +
                            $"{biosVer[0]}\n"  +
                            $"{biosVer[1]}\n"  ;

                    foreach (var ch in biosChrs) {
                        if      (ch >= 0  && ch <= 39)  yield return wmiBiosCharacteristics[ch];
                        else if (ch >= 40 && ch <= 47)  yield return $"{ch}-Reserved for BIOS vendor";
                        else if (ch >= 48 && ch <= 63)  yield return $"{ch}-Reserved for system vendor";
                    }
                }
            }
        }

        private static readonly string[] wmiBiosCharacteristics = {
          "00-Reserved",
          "01-Reserved",
          "02-Unknown",
          "03-BIOS Characteristics Not Supported",
          "04-ISA is supported",
          "05-MCA is supported",
          "06-EISA is supported",
          "07-PCI is supported",
          "08-PC Card (PCMCIA) is supported",
          "09-Plug and Play is supported",
          "10-APM is supported",
          "11-BIOS is Upgradable (Flash)",
          "12-BIOS shadowing is allowed",
          "13-VL-VESA is supported",
          "14-ESCD support is available",
          "15-Boot from CD is supported",
          "16-Selectable Boot is supported",
          "17-BIOS ROM is socketed",
          "18-Boot From PC Card (PCMCIA) is supported",
          "19-EDD (Enhanced Disk Drive) Specification is supported",
          "20-Int 13h - Japanese Floppy for NEC 9800 1.2mb (3.5, 1k Bytes/Sector, 360 RPM) is supported",
          "21-Int 13h - Japanese Floppy for Toshiba 1.2mb (3.5, 360 RPM) is supported",
          "22-Int 13h - 5.25 / 360 KB Floppy Services are supported",
          "23-Int 13h - 5.25 /1.2MB Floppy Services are supported",
          "24-Int 13h - 3.5 / 720 KB Floppy Services are supported",
          "25-Int 13h - 3.5 / 2.88 MB Floppy Services are supported",
          "26-Int 5h, Print Screen Service is supported",
          "27-Int 9h, 8042 Keyboard services are supported",
          "28-Int 14h, Serial Services are supported",
          "29-Int 17h, printer services are supported",
          "30-Int 10h, CGA/Mono Video Services are supported",
          "31-NEC PC-98",
          "32-ACPI supported",
          "33-USB Legacy is supported",
          "34-AGP is supported",
          "35-I2O boot is supported",
          "36-LS-120 boot is supported",
          "37-ATAPI ZIP Drive boot is supported",
          "38-1394 boot is supported",
          "39-Smart Battery supported"
        };

    }
}
