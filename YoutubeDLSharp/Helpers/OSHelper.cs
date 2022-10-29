using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDLSharp.Helpers
{
    internal static class OSHelper
    {
        public static bool IsWindows { get => GetOSVersion() == OSVersion.Windows; }

        /// <summary>
        /// Gets the <see cref="OSVersion"/> depending on what platform you are on
        /// </summary>
        /// <returns>Returns the OS Version</returns>
        /// <exception cref="Exception"></exception>
        internal static OSVersion GetOSVersion()
        {
#if NET45
            return OSVersion.Windows;
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSVersion.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSVersion.OSX;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSVersion.Linux;
            }
            else { throw new Exception("Your OS isn't supported"); }
#endif
        }
    }
    internal enum OSVersion
    {
        Windows,
        OSX,
        Linux
    }
}