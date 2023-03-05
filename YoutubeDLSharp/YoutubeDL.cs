using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using YoutubeDLSharp.Helpers;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp.Options;

namespace YoutubeDLSharp
{
    /// <summary>
    /// A class providing methods for downloading videos using yt-dlp.
    /// </summary>
    public class YoutubeDL
    {
        private static readonly Regex rgxFile = new Regex(@"^outfile:\s\""?(.*)\""?", RegexOptions.Compiled);
        private static Regex rgxFilePostProc = new Regex(@"\[download\] Destination: [a-zA-Z]:\\\S+\.\S{3,}", RegexOptions.Compiled);

        protected ProcessRunner runner;

        /// <summary>
        /// Path to the yt-dlp executable.
        /// </summary>
        public string YoutubeDLPath { get; set; } = Utils.YtDlpBinaryName;
        /// <summary>
        /// Path to the FFmpeg executable.
        /// </summary>
        public string FFmpegPath { get; set; } = Utils.FfmpegBinaryName;
        /// <summary>
        /// Path of the folder where items will be downloaded to.
        /// </summary>
        public string OutputFolder { get; set; } = Environment.CurrentDirectory;
        /// <summary>
        /// Template of the name of the downloaded file on yt-dlp style.
        /// See https://github.com/yt-dlp/yt-dlp#output-template.
        /// </summary>
        public string OutputFileTemplate { get; set; } = "%(title)s [%(id)s].%(ext)s";
        /// <summary>
        /// If set to true, file names a re restricted to ASCII characters.
        /// </summary>
        public bool RestrictFilenames { get; set; } = false;
        /// <summary>
        /// If set to true, force overwriting of files.
        /// </summary>
        public bool OverwriteFiles { get; set; } = true;
        /// <summary>
        /// If set to true, download errors are ignored and downloading is continued.
        /// </summary>
        public bool IgnoreDownloadErrors { get; set; } = true;

        /// <summary>
        /// Gets the product version of the yt-dlp executable file.
        /// </summary>
        public string Version
            => FileVersionInfo.GetVersionInfo(Utils.GetFullPath(YoutubeDLPath)).FileVersion;

        /// <summary>
        /// Creates a new instance of the YoutubeDL class.
        /// </summary>
        /// <param name="maxNumberOfProcesses">The maximum number of concurrent yt-dlp processes.</param>
        public YoutubeDL(byte maxNumberOfProcesses = 4)
        {
            runner = new ProcessRunner(maxNumberOfProcesses);
        }

        /// <summary>
        /// Sets the maximal number of parallel download processes.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task SetMaxNumberOfProcesses(byte count) => await runner.SetTotalCount(count);

        #region Process methods

        /// <summary>
        /// Runs yt-dlp with the given option set.
        /// </summary>
        /// <param name="urls">The video URLs passed to yt-dlp.</param>
        /// <param name="options">The OptionSet of yt-dlp options.</param>
        /// <param name="ct">A CancellationToken used to cancel the process.</param>
        /// <returns>A RunResult object containing the output of yt-dlp as an array of string.</returns>
        public async Task<RunResult<string[]>> RunWithOptions(string[] urls, OptionSet options, CancellationToken ct)
        {
            var output = new List<string>();
            var process = new YoutubeDLProcess(YoutubeDLPath);
            process.OutputReceived += (o, e) => output.Add(e.Data);
            (int code, string[] errors) = await runner.RunThrottled(process, urls, options, ct);
            return new RunResult<string[]>(code == 0, errors, output.ToArray());
        }

        /// <summary>
        /// Runs yt-dlp with the given option set and additional parameters.
        /// </summary>
        /// <param name="url">The video URL passed to yt-dlp.</param>
        /// <param name="options">The OptionSet of yt-dlp options.</param>
        /// <param name="ct">A CancellationToken used to cancel the process.</param>
        /// <param name="progress">A progress provider used to get download progress information.</param>
        /// <param name="output">A progress provider used to capture the standard output.</param>
        /// <param name="showArgs">When true, outputs full download arguments</param>
        /// <returns>A RunResult object containing the path to the downloaded and converted video.</returns>
        public async Task<RunResult<string>> RunWithOptions(string url, OptionSet options, CancellationToken ct = default,
            IProgress<DownloadProgress> progress = null, IProgress<string> output = null, bool showArgs = true)
        {
            string outFile = string.Empty;
            var process = new YoutubeDLProcess(YoutubeDLPath);
            if (showArgs)
                output?.Report($"Arguments: {process.ConvertToArgs(new[] { url }, options)}\n");
            else
                output?.Report($"Starting Download: {url}");
            process.OutputReceived += (o, e) =>
            {
                var match = rgxFilePostProc.Match(e.Data);
                if (match.Success)
                {
                    outFile = match.Groups[0].ToString().Replace("[download] Destination:", "").Replace(" ", "");
                    progress?.Report(new DownloadProgress(DownloadState.Success, data: outFile));
                }
                output?.Report(e.Data);
            };
            (int code, string[] errors) = await runner.RunThrottled(process, new[] { url }, options, ct, progress);
            return new RunResult<string>(code == 0, errors, outFile);
        }

        /// <summary>
        /// Runs an update of yt-dlp.
        /// </summary>
        /// <returns>The output of yt-dlp as string.</returns>
        public async Task<string> RunUpdate()
        {
            string output = String.Empty;
            var process = new YoutubeDLProcess(YoutubeDLPath);
            process.OutputReceived += (o, e) => output = e.Data;
            await process.RunAsync(null, new OptionSet() { Update = true });
            return output;
        }

        /// <summary>
        /// Runs a fetch of information for the given video without downloading the video.
        /// </summary>
        /// <param name="url">The URL of the video to fetch information for.</param>
        /// <param name="ct">A CancellationToken used to cancel the process.</param>
        /// <param name="flat">If set to true, does not extract information for each video in a playlist.</param>
        /// <param name="fetchComments">If set to true, fetch comment data for the given video.</param>
        /// <param name="overrideOptions">Override options of the default option set for this run.</param>
        /// <returns>A RunResult object containing a VideoData object with the requested video information.</returns>
        public async Task<RunResult<VideoData>> RunVideoDataFetch(string url,
            CancellationToken ct = default,
            bool flat = true,
            bool fetchComments = false,
            OptionSet overrideOptions = null)
        {
            var opts = GetDownloadOptions();
            opts.DumpSingleJson = true;
            opts.FlatPlaylist = flat;
            opts.WriteComments = fetchComments;
            if (overrideOptions != null)
            {
                opts = opts.OverrideOptions(overrideOptions);
            }
            VideoData videoData = null;
            var process = new YoutubeDLProcess(YoutubeDLPath);
            process.OutputReceived += (o, e) => videoData = JsonConvert.DeserializeObject<VideoData>(e.Data);
            (int code, string[] errors) = await runner.RunThrottled(process, new[] { url }, opts, ct);
            return new RunResult<VideoData>(code == 0, errors, videoData);
        }

        /// <summary>
        /// Runs a download of the specified video with an optional conversion afterwards.
        /// </summary>
        /// <param name="url">The URL of the video to be downloaded.</param>
        /// <param name="format">A format selection string in yt-dlp style.</param>
        /// <param name="mergeFormat">If a merge is required, the container format of the merged downloads.</param>
        /// <param name="recodeFormat">The video format the output will be recoded to after download.</param>
        /// <param name="ct">A CancellationToken used to cancel the download.</param>
        /// <param name="progress">A progress provider used to get download progress information.</param>
        /// <param name="output">A progress provider used to capture the standard output.</param>
        /// <param name="overrideOptions">Override options of the default option set for this run.</param>
        /// <returns>A RunResult object containing the path to the downloaded and converted video.</returns>
        public async Task<RunResult<string>> RunVideoDownload(string url,
            string format = "bestvideo+bestaudio/best",
            DownloadMergeFormat mergeFormat = DownloadMergeFormat.Unspecified,
            VideoRecodeFormat recodeFormat = VideoRecodeFormat.None,
            CancellationToken ct = default, IProgress<DownloadProgress> progress = null,
            IProgress<string> output = null, OptionSet overrideOptions = null)
        {
            var opts = GetDownloadOptions();
            opts.Format = format;
            opts.MergeOutputFormat = mergeFormat;
            opts.RecodeVideo = recodeFormat;
            if (overrideOptions != null)
            {
                opts = opts.OverrideOptions(overrideOptions);
            }
            string outputFile = String.Empty;
            var process = new YoutubeDLProcess(YoutubeDLPath);
            // Report the used ytdl args
            output?.Report($"Arguments: {process.ConvertToArgs(new[] { url }, opts)}\n");
            process.OutputReceived += (o, e) =>
            {
                var match = rgxFile.Match(e.Data);
                if (match.Success)
                {
                    outputFile = match.Groups[1].ToString().Trim('"');
                    progress?.Report(new DownloadProgress(DownloadState.Success, data: outputFile));
                }
                output?.Report(e.Data);
            };
            (int code, string[] errors) = await runner.RunThrottled(process, new[] { url }, opts, ct, progress);
            return new RunResult<string>(code == 0, errors, outputFile);
        }

        /// <summary>
        /// Runs a download of the specified video playlist with an optional conversion afterwards.
        /// </summary>
        /// <param name="url">The URL of the playlist to be downloaded.</param>
        /// <param name="start">The index of the first playlist video to download (starting at 1).</param>
        /// <param name="end">The index of the last playlist video to dowload (if null, download to end).</param>
        /// <param name="items">An array of indices of playlist video to download.</param>
        /// <param name="format">A format selection string in yt-dlp style.</param>
        /// <param name="recodeFormat">The video format the output will be recoded to after download.</param>
        /// <param name="ct">A CancellationToken used to cancel the download.</param>
        /// <param name="progress">A progress provider used to get download progress information.</param>
        /// <param name="output">A progress provider used to capture the standard output.</param>
        /// <param name="overrideOptions">Override options of the default option set for this run.</param>
        /// <returns>A RunResult object containing the paths to the downloaded and converted videos.</returns>
        public async Task<RunResult<string[]>> RunVideoPlaylistDownload(string url,
            int? start = 1, int? end = null,
            int[] items = null,
            string format = "bestvideo+bestaudio/best",
            VideoRecodeFormat recodeFormat = VideoRecodeFormat.None,
            CancellationToken ct = default, IProgress<DownloadProgress> progress = null,
            IProgress<string> output = null, OptionSet overrideOptions = null)
        {
            var opts = GetDownloadOptions();
            opts.NoPlaylist = false;
            opts.PlaylistStart = start;
            opts.PlaylistEnd = end;
            if (items != null)
                opts.PlaylistItems = String.Join(",", items);
            opts.Format = format;
            opts.RecodeVideo = recodeFormat;
            if (overrideOptions != null)
            {
                opts = opts.OverrideOptions(overrideOptions);
            }
            var outputFiles = new List<string>();
            var process = new YoutubeDLProcess(YoutubeDLPath);
            // Report the used ytdl args
            output?.Report($"Arguments: {process.ConvertToArgs(new[] { url }, opts)}\n");
            process.OutputReceived += (o, e) =>
            {
                var match = rgxFile.Match(e.Data);
                if (match.Success)
                {
                    var file = match.Groups[1].ToString().Trim('"');
                    outputFiles.Add(file);
                    progress?.Report(new DownloadProgress(DownloadState.Success, data: file));
                }
                output?.Report(e.Data);
            };
            (int code, string[] errors) = await runner.RunThrottled(process, new[] { url }, opts, ct, progress);
            return new RunResult<string[]>(code == 0, errors, outputFiles.ToArray());
        }

        /// <summary>
        /// Runs a download of the specified video with and converts it to an audio format afterwards.
        /// </summary>
        /// <param name="url">The URL of the video to be downloaded.</param>
        /// <param name="format">The audio format the video will be converted to after downloaded.</param>
        /// <param name="ct">A CancellationToken used to cancel the download.</param>
        /// <param name="progress">A progress provider used to get download progress information.</param>
        /// <param name="output">A progress provider used to capture the standard output.</param>
        /// <param name="overrideOptions">Override options of the default option set for this run.</param>
        /// <returns>A RunResult object containing the path to the downloaded and converted video.</returns>
        public async Task<RunResult<string>> RunAudioDownload(string url, AudioConversionFormat format = AudioConversionFormat.Best,
            CancellationToken ct = default, IProgress<DownloadProgress> progress = null,
            IProgress<string> output = null, OptionSet overrideOptions = null)
        {
            var opts = GetDownloadOptions();
            opts.Format = "bestaudio/best";
            opts.ExtractAudio = true;
            opts.AudioFormat = format;
            if (overrideOptions != null)
            {
                opts = opts.OverrideOptions(overrideOptions);
            }
            string outputFile = String.Empty;
            var error = new List<string>();
            var process = new YoutubeDLProcess(YoutubeDLPath);
            // Report the used ytdl args
            output?.Report($"Arguments: {process.ConvertToArgs(new[] { url }, opts)}\n");
            process.OutputReceived += (o, e) =>
            {
                var match = rgxFile.Match(e.Data);
                if (match.Success)
                {
                    outputFile = match.Groups[1].ToString().Trim('"');
                    progress?.Report(new DownloadProgress(DownloadState.Success, data: outputFile));
                }
                output?.Report(e.Data);
            };
            (int code, string[] errors) = await runner.RunThrottled(process, new[] { url }, opts, ct, progress);
            return new RunResult<string>(code == 0, errors, outputFile);
        }

        /// <summary>
        /// Runs a download of the specified video playlist and converts all videos to an audio format afterwards.
        /// </summary>
        /// <param name="url">The URL of the playlist to be downloaded.</param>
        /// <param name="start">The index of the first playlist video to download (starting at 1).</param>
        /// <param name="end">The index of the last playlist video to dowload (if null, download to end).</param>
        /// <param name="items">An array of indices of playlist video to download.</param>
        /// <param name="format">The audio format the videos will be converted to after downloaded.</param>
        /// <param name="ct">A CancellationToken used to cancel the download.</param>
        /// <param name="progress">A progress provider used to get download progress information.</param>
        /// <param name="output">A progress provider used to capture the standard output.</param>
        /// <param name="overrideOptions">Override options of the default option set for this run.</param>
        /// <returns>A RunResult object containing the paths to the downloaded and converted videos.</returns>
        public async Task<RunResult<string[]>> RunAudioPlaylistDownload(string url,
            int? start = 1, int? end = null,
            int[] items = null, AudioConversionFormat format = AudioConversionFormat.Best,
            CancellationToken ct = default, IProgress<DownloadProgress> progress = null,
            IProgress<string> output = null, OptionSet overrideOptions = null)
        {
            var outputFiles = new List<string>();
            var opts = GetDownloadOptions();
            opts.NoPlaylist = false;
            opts.PlaylistStart = start;
            opts.PlaylistEnd = end;
            if (items != null)
                opts.PlaylistItems = String.Join(",", items);
            opts.Format = "bestaudio/best";
            opts.ExtractAudio = true;
            opts.AudioFormat = format;
            if (overrideOptions != null)
            {
                opts = opts.OverrideOptions(overrideOptions);
            }
            var process = new YoutubeDLProcess(YoutubeDLPath);
            // Report the used ytdl args
            output?.Report($"Arguments: {process.ConvertToArgs(new[] { url }, opts)}\n");
            process.OutputReceived += (o, e) =>
            {
                var match = rgxFile.Match(e.Data);
                if (match.Success)
                {
                    var file = match.Groups[1].ToString().Trim('"');
                    outputFiles.Add(file);
                    progress?.Report(new DownloadProgress(DownloadState.Success, data: file));
                }
                output?.Report(e.Data);
            };
            (int code, string[] errors) = await runner.RunThrottled(process, new[] { url }, opts, ct, progress);
            return new RunResult<string[]>(code == 0, errors, outputFiles.ToArray());
        }

        /// <summary>
        /// Returns an option set with default options used for most downloading operations.
        /// </summary>
        protected virtual OptionSet GetDownloadOptions()
        {
            return new OptionSet()
            {
                IgnoreErrors = this.IgnoreDownloadErrors,
                IgnoreConfig = true,
                NoPlaylist = true,
                Downloader = "m3u8:native",
                DownloaderArgs = "ffmpeg:-nostats -loglevel 0",
                Output = Path.Combine(OutputFolder, OutputFileTemplate),
                RestrictFilenames = this.RestrictFilenames,
                ForceOverwrites = this.OverwriteFiles,
                NoOverwrites = !this.OverwriteFiles,
                NoPart = true,
                FfmpegLocation = Utils.GetFullPath(this.FFmpegPath),
                Exec = "echo outfile: {}"
            };
        }

        #endregion        
    }
}
