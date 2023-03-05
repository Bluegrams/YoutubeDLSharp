using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeDLSharp.Helpers;

namespace YoutubeDLSharp
{
    /// <summary>
    /// Utility methods.
    /// </summary>
    public static class Utils
    {
        private static readonly HttpClient _client = new HttpClient();

        private static readonly Regex rgxTimestamp = new Regex("[0-9]+(?::[0-9]+)+", RegexOptions.Compiled);
        private static readonly Dictionary<char, string> accentChars
            = "ÂÃÄÀÁÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖŐØŒÙÚÛÜŰÝÞßàáâãäåæçèéêëìíîïðñòóôõöőøœùúûüűýþÿ"
                .Zip(new[] { "A","A","A","A","A","A","AE","C","E","E","E","E","I","I","I","I","D","N",
                    "O","O","O","O","O","O","O","OE","U","U","U","U","U","Y","P","ss",
                    "a","a","a","a","a","a","ae","c","e","e","e","e","i","i","i","i","o","n",
                    "o","o","o","o","o","o","o","oe","u","u","u","u","u","y","p","y"},
                    (c, s) => new { Key = c, Val = s }).ToDictionary(o => o.Key, o => o.Val);

        /// <summary>
        /// Sanitize a string to be a valid file name.
        /// Ported from:
        /// https://github.com/ytdl-org/youtube-dl/blob/33c1c7d80fd99024879a5f087b55b24374385e43/youtube_dl/utils.py#L2067
        /// </summary>
        /// <returns></returns>
        public static string Sanitize(string s, bool restricted = false)
        {
            rgxTimestamp.Replace(s, m => m.Groups[0].Value.Replace(':', '_'));
            var result = String.Join("", s.Select(c => sanitizeChar(c, restricted)));
            result = result.Replace("__", "_").Trim('_');
            if (restricted && result.StartsWith("-_"))
                result = result.Substring(2);
            if (result.StartsWith("-"))
                result = "_" + result.Substring(1);
            result = result.TrimStart('.');
            if (String.IsNullOrWhiteSpace(result))
                result = "_";
            return result;
        }

        private static string sanitizeChar(char c, bool restricted)
        {
            if (restricted && accentChars.ContainsKey(c))
                return accentChars[c];
            else if (c == '?' || c < 32 || c == 127)
                return "";
            else if (c == '"')
                return restricted ? "" : "\'";
            else if (c == ':')
                return restricted ? "_-" : " -";
            else if ("\\/|*<>".Contains(c))
                return "_";
            else if (restricted && "!&\'()[]{}$;`^,# ".Contains(c))
                return "_";
            else if (restricted && c > 127)
                return "_";
            else return c.ToString();
        }

        /// <summary>
        /// Returns the absolute path for the specified path string.
        /// Also searches the environment's PATH variable.
        /// </summary>
        /// <param name="fileName">The relative path string.</param>
        /// <returns>The absolute path or null if the file was not found.</returns>
        public static string GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
                return Path.GetFullPath(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var p in values.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(p, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }

#region Download Helpers

        public static string YtDlpBinaryName => GetYtDlpBinaryName();
        public static string FfmpegBinaryName => GetFfmpegBinaryName();
        public static string FfprobeBinaryName => GetFfprobeBinaryName();

        public static async Task DownloadBinaries(bool skipExisting = true, string directoryPath = "")
        {
            if (skipExisting)
            {
                if(!File.Exists(Path.Combine(directoryPath, GetYtDlpBinaryName())))
                {
                    await DownloadYtDlp(directoryPath);
                }
                if (!File.Exists(Path.Combine(directoryPath, GetFfmpegBinaryName())))
                {
                    await DownloadFFmpeg(directoryPath);
                }
                if (!File.Exists(Path.Combine(directoryPath, GetFfprobeBinaryName())))
                {
                    await DownloadFFprobe(directoryPath);
                }
            }
            else
            {
                await DownloadYtDlp(directoryPath);
                await DownloadFFmpeg(directoryPath);
                await DownloadFFprobe(directoryPath);
            }            
        }

        private static string GetYtDlpDownloadUrl()
        {
            const string BASE_GITHUB_URL = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp";

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
            return downloadUrl;
        }

        private static string GetYtDlpBinaryName()
        {
            string ytdlpDownloadPath = GetYtDlpDownloadUrl();
            return Path.GetFileName(ytdlpDownloadPath);            
        }

        private static string GetFfmpegBinaryName()
        {
            switch (OSHelper.GetOSVersion())
            {
                case OSVersion.Windows:
                    return "ffmpeg.exe";
                case OSVersion.OSX:
                case OSVersion.Linux:
                    return "ffmpeg";
                default:
                    throw new Exception("Your OS isn't supported");
            }            
        }

        private static string GetFfprobeBinaryName()
        {
            switch (OSHelper.GetOSVersion())
            {
                case OSVersion.Windows:
                    return "ffprobe.exe";
                case OSVersion.OSX:
                case OSVersion.Linux:
                    return "ffprobe";
                default:
                    throw new Exception("Your OS isn't supported");
            }
        }

        public static async Task DownloadYtDlp(string directoryPath = "")
        {
            string downloadUrl = GetYtDlpDownloadUrl();

            if (string.IsNullOrEmpty(directoryPath)) { directoryPath = Directory.GetCurrentDirectory(); }

            var downloadLocation = Path.Combine(directoryPath, Path.GetFileName(downloadUrl));
            var data = await DownloadFileBytesAsync(downloadUrl);
            File.WriteAllBytes(downloadLocation, data);
        }

        public static async Task DownloadFFmpeg(string directoryPath = "")
        {
            await FFDownloader(directoryPath, FFmpegApi.BinaryType.FFmpeg);
        }

        public static async Task DownloadFFprobe(string directoryPath = "")
        {
            await FFDownloader(directoryPath, FFmpegApi.BinaryType.FFprobe);
        }

        

        private static async Task FFDownloader(string directoryPath = "", FFmpegApi.BinaryType binary = FFmpegApi.BinaryType.FFmpeg)
        {
            if (string.IsNullOrEmpty(directoryPath)) { directoryPath = Directory.GetCurrentDirectory(); }
            const string FFMPEG_API_URL = "https://ffbinaries.com/api/v1/version/latest";

            var ffmpegVersion = JsonConvert.DeserializeObject<FFmpegApi.Root>(await (await _client.GetAsync(FFMPEG_API_URL)).Content.ReadAsStringAsync());

            FFmpegApi.OsBinVersion ffContent;
            switch (OSHelper.GetOSVersion())
            {
                case OSVersion.Windows:
                    ffContent = ffmpegVersion?.Bin.Windows64;
                    break;
                case OSVersion.OSX:
                    ffContent = ffmpegVersion?.Bin.Osx64;
                    break;
                case OSVersion.Linux:
                    ffContent= ffmpegVersion?.Bin.Linux64;
                    break;
                default:
                    throw new NotImplementedException("Your OS isn't supported");
            }

            string downloadUrl = binary == FFmpegApi.BinaryType.FFmpeg ? ffContent.Ffmpeg : ffContent.Ffprobe;
            var dataBytes = await DownloadFileBytesAsync(downloadUrl);
            using (var stream = new MemoryStream(dataBytes))
            {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    if (archive.Entries.Count > 0)
                    {
                        archive.Entries[0].ExtractToFile(Path.Combine(directoryPath, archive.Entries[0].FullName), true);
                    }
                }                
            };            
        }

        private static async Task<byte[]> DownloadFileBytesAsync(string uri)
        {
            if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri _))
                throw new InvalidOperationException("URI is invalid.");

            byte[] fileBytes = await _client.GetByteArrayAsync(uri);
            return fileBytes;
        }


        internal class FFmpegApi
        {
            public class Root
            {
                [JsonProperty("version")]
                public string Version { get; set; }

                [JsonProperty("permalink")]
                public string Permalink { get; set; }

                [JsonProperty("bin")]
                public Bin Bin { get; set; }
            }

            public class Bin
            {
                [JsonProperty("windows-64")]
                public OsBinVersion Windows64 { get; set; }

                [JsonProperty("linux-64")]
                public OsBinVersion Linux64 { get; set; }

                [JsonProperty("osx-64")]
                public OsBinVersion Osx64 { get; set; }
            }

            public class OsBinVersion
            {
                [JsonProperty("ffmpeg")]
                public string Ffmpeg { get; set; }

                [JsonProperty("ffprobe")]
                public string Ffprobe { get; set; }
            }

            public enum BinaryType
            {
                [EnumMember(Value = "ffmpeg")]
                FFmpeg,
                [EnumMember(Value = "ffprobe")]
                FFprobe
            }
        }
#endregion
    }
}
