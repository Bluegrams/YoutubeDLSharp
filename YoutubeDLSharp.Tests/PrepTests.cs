using System.IO;
using System.Threading.Tasks;

namespace YoutubeDLSharp.Tests;

internal static class PrepTests
{
    private static bool _didDownloadBinaries;
    internal static async Task DownloadBinaries()
    {
        if (_didDownloadBinaries == false)
        {
            if (!File.Exists("yt-dlp"))
            {
                await YoutubeDl.DownloadYtDlpBinary();
            }
            if (!File.Exists("ffmpeg"))
            {
                await YoutubeDl.DownloadFFmpegBinary();
            }
            _didDownloadBinaries = true;
        }
    }
}