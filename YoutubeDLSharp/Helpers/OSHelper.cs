using System;
using System.Runtime.InteropServices;

namespace YoutubeDLSharp.Helpers;

internal static class OsHelper
{
    public static bool IsWindows => GetOsVersion() == OsVersion.Windows;

    /// <summary>
    /// Gets the <see cref="OsVersion"/> depending on what platform you are on
    /// </summary>
    /// <returns>Returns the OS Version</returns>
    /// <exception cref="Exception"></exception>
    internal static OsVersion GetOsVersion()
    { 
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return OsVersion.Windows;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return OsVersion.Osx;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return OsVersion.Linux;
        }

        throw new Exception("Your OS isn't supported");
    }
}
internal enum OsVersion
{
    Windows,
    Osx,
    Linux
}