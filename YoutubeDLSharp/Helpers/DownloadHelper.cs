using System;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Net.Http;

namespace YoutubeDLSharp.Helpers
{
    internal static class DownloadHelper
    {
        private const string BASE_GITHUB_URL = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp";
        private const string FFMPEG_API_URL = "https://ffbinaries.com/api/v1/version/latest";
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// Downloads the YT-DLP binary depending on OS
        /// </summary>
        /// <param name="directoryPath">The optional directory of where it should be saved to</param>
        /// <exception cref="Exception"></exception>
        internal static async Task DownloadYtDlp(string directoryPath = "")
        {
            if (string.IsNullOrEmpty(directoryPath)) { directoryPath = Directory.GetCurrentDirectory(); }

            string downloadUrl;
            switch (OSHelper.GetOSVersion())
            {
                case OSVersion.Windows:
                    downloadUrl = $"{BASE_GITHUB_URL}.exe";
                    break;
                case OSVersion.OSX:
                    downloadUrl = $"{BASE_GITHUB_URL}_macos";
                    break;
                case OSVersion.Linux:
                    downloadUrl = BASE_GITHUB_URL;
                    break;
                default:
                    throw new Exception("Your OS isn't supported");
            }

            var downloadLocation = Path.Combine(directoryPath, Path.GetFileName(downloadUrl));
            var data = await DownloadFileBytesAsync(downloadUrl);
            File.WriteAllBytes(downloadLocation, data);
        }

        /// <summary>
        /// Downloads the FFmpeg binary depending on the OS
        /// </summary>
        /// <param name="directoryPath">The optional directory of where it should be saved to</param>
        /// <exception cref="Exception"></exception>
        internal static async Task DownloadFFmpeg(string directoryPath = "")
        {
            if (string.IsNullOrEmpty(directoryPath)) { directoryPath = Directory.GetCurrentDirectory(); }

            string response = await client.GetStringAsync(FFMPEG_API_URL);
            var result = JsonConvert.DeserializeObject<JToken>(response);

            string ffmpegURL;
            switch (OSHelper.GetOSVersion())
            {
                case OSVersion.Windows:
                    ffmpegURL = result["bin"]["windows-64"]["ffmpeg"].ToString();
                    break;
                case OSVersion.OSX:
                    ffmpegURL = result["bin"]["osx-64"]["ffmpeg"].ToString();
                    break;
                case OSVersion.Linux:
                    ffmpegURL = result["bin"]["linux-64"]["ffmpeg"].ToString();
                    break;
                default:
                    throw new Exception("Your OS isn't supported");
            }
            var data = await DownloadFileBytesAsync(ffmpegURL);

            using (var stream = new MemoryStream(data))
            {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read, true))
                {
                    if (archive.Entries.Count > 0)
                    {
                        archive.Entries[0].ExtractToFile(Path.Combine(directoryPath, archive.Entries[0].FullName), true);
                    }
                }
            }

        }

        /// <summary>
        /// Downloads a file from the specified URI
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>Returns a byte array of the file that was downloaded</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static async Task<byte[]> DownloadFileBytesAsync(string uri)
        {
            if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri uriResult))
                throw new InvalidOperationException("URI is invalid.");

            byte[] fileBytes = await client.GetByteArrayAsync(uri);
            return fileBytes;
        }

    }

}