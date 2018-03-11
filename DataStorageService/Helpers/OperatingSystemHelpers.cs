using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DataStorageService.Helpers
{
    public static class OperatingSystemHelpers
    {
        public static bool IsOSWindows =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool IsOSMac =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static bool IsOSLinux =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static string SystemSlash {
        get
            {
                if (IsOSWindows)
                    return "\\";
                else
                    return "/";
            }
        }
        
    }
}
