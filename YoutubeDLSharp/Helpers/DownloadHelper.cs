using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Net.Http;

namespace YoutubeDLSharp.Helpers
{
    internal class DownloadHelper
    {
        /// <summary>
        /// Downloads the YT-DLP binary depending on OS
        /// </summary>
        /// <param name="directoryPath">The optional directory of where it should be saved to</param>
        /// <exception cref="Exception"></exception>
        internal static void DownloadYtDlp(string directoryPath = "")
        {
            const string BASE_GITHUB_URL = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp";
            string downloadUrl = "";
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

            if (string.IsNullOrEmpty(directoryPath)) { directoryPath = Directory.GetCurrentDirectory(); }

            var downloadLocation = Path.Combine(directoryPath, Path.GetFileName(downloadUrl));
            var data = Task.Run(() => DownloadFileBytesAsync(downloadUrl)).Result;
            File.WriteAllBytes(downloadLocation, data);
        }

        /// <summary>
        /// Downloads the FFmpeg binary depending on the OS
        /// </summary>
        /// <param name="directoryPath">The optional directory of where it should be saved to</param>
        /// <exception cref="Exception"></exception>
        internal static void DownloadFFmpeg(string directoryPath = "")
        {
            if (string.IsNullOrEmpty(directoryPath)) { directoryPath = Directory.GetCurrentDirectory(); }
            const string FFMPEG_API_URL = "https://ffbinaries.com/api/v1/version/latest";
            var httpRequest = (HttpWebRequest)WebRequest.Create(FFMPEG_API_URL);
            httpRequest.Accept = "application/json";

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            string jsonData;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                jsonData = streamReader.ReadToEnd();
            }
            var result = JsonConvert.DeserializeObject<JToken>(jsonData);

            string ffmpegURL = "";
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
            var data = Task.Run(() => DownloadFileBytesAsync(ffmpegURL)).Result;

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
            Uri uriResult;

            if (!Uri.TryCreate(uri, UriKind.Absolute, out uriResult))
                throw new InvalidOperationException("URI is invalid.");

            var httpClient = new HttpClient();
            byte[] fileBytes = await httpClient.GetByteArrayAsync(uri);
            return fileBytes;
        }

    }

}