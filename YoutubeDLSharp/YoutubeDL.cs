using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using YoutubeDLSharp.Helpers;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp.Options;
using Xabe.FFmpeg;

namespace YoutubeDLSharp
{
    /// <summary>
    /// A class providing methods for downloading videos using youtube-dl.
    /// </summary>
    public class YoutubeDL
    {
        private static Regex rgxFile = new Regex("echo\\s\\\"?(.*)\\\"?", RegexOptions.Compiled);
        private static Regex rgxFilePostProc = new Regex(@"\[download\] Destination: [a-zA-Z]:\\\S+\.\S{3,}", RegexOptions.Compiled);

        protected ProcessRunner runner;

        /// <summary>
        /// Path to the youtube-dl executable.
        /// </summary>
        public string YoutubeDLPath { get; set; } = "youtube-dl.exe";

        private string ffmpegPath = "ffmpeg.exe";
        /// <summary>
        /// Path to the FFmpeg executable.
        /// </summary>
        public string FFmpegPath 
        {        
            get { return ffmpegPath; }
            set { ffmpegPath = value; }
        }

        private string ffmpegExecutablesPath = "";
        /// <summary>
        /// Path to FFmpeg and FFprobe executables
        /// </summary>
        public string FFmpegExecutablesPath
        {
            get { return ffmpegExecutablesPath; }
            set
            {
                ffmpegExecutablesPath = value;
                ffmpegPath = Path.Combine(ffmpegExecutablesPath, "ffmpeg.exe");
                FFmpeg.SetExecutablesPath(ffmpegExecutablesPath);
            }
        }
        /// <summary>
        /// Path of the folder where items will be downloaded to.
        /// </summary>
        public string OutputFolder { get; set; } = Environment.CurrentDirectory;
        /// <summary>
        /// Template of the name of the downloaded file on youtube-dl style.
        /// See https://github.com/ytdl-org/youtube-dl#output-template.
        /// </summary>
        public string OutputFileTemplate { get; set; } = "%(title)s.%(ext)s";
        /// <summary>
        /// If set to true, file names a re restricted to ASCII characters.
        /// </summary>
        public bool RestrictFilenames { get; set; } = false;

        /* TODO IMPORTANT This flag does not work fully as expected:
         * Does not guarantee overwrites (see https://github.com/ytdl-org/youtube-dl/pull/20405)
         * Always overwrites post-processed files
         * (see https://github.com/ytdl-org/youtube-dl/issues/5173, https://github.com/ytdl-org/youtube-dl/issues/333)
         */
        public bool OverwriteFiles { get; set; } = true;

        /// <summary>
        /// If set to true, download errors are ignored and downloading is continued.
        /// </summary>
        public bool IgnoreDownloadErrors { get; set; } = true;

        /// <summary>
        /// Gets the product version of the youtube-dl executable file.
        /// </summary>
        public string Version
            => FileVersionInfo.GetVersionInfo(Utils.GetFullPath(YoutubeDLPath)).FileVersion;

        /// <summary>
        /// Creates a new instance of the YoutubeDL class.
        /// </summary>
        /// <param name="maxNumberOfProcesses">The maximum number of concurrent youtube-dl processes.</param>
        /// <param name="ytdlPath">The path to youtube-dl.exe</param>
        /// <param name="ffmpegExecutablesPath">The path to the directory where ffmpeg executables are located</param>
        public YoutubeDL(byte maxNumberOfProcesses = 4, string ytdlPath = "", string ffmpegExecutablesPath = "")
        {
            runner = new ProcessRunner(maxNumberOfProcesses);
            if (!string.IsNullOrEmpty(ytdlPath))
                YoutubeDLPath = ytdlPath;
            if (!string.IsNullOrEmpty(ffmpegExecutablesPath))
                FFmpegExecutablesPath = ffmpegExecutablesPath;
        }

        /// <summary>
        /// Sets the maximal number of parallel download processes.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task SetMaxNumberOfProcesses(byte count) => await runner.SetTotalCount(count);

        #region Process methods

        /// <summary>
        /// Runs youtube-dl with the given option set.
        /// </summary>
        /// <param name="urls">The video URLs passed to youtube-dl.</param>
        /// <param name="options">The OptionSet of youtube-dl options.</param>
        /// <param name="ct">A CancellationToken used to cancel the process.</param>
        /// <param name="progress">A progress provider used to get download progress information.</param>
        /// <param name="output">A progress provider used to capture the standard output.</param>
        /// <returns>A RunResult object containing the output of youtube-dl as an array of string.</returns>
        public async Task<RunResult<string[]>> RunWithOptions(string[] urls, OptionSet options, CancellationToken ct,
            IProgress<DownloadProgress> progress = null, IProgress<string> output = null)
        {
            var outputList = new List<string>();
            var process = new YoutubeDLProcess(YoutubeDLPath);
            process.OutputReceived += (o, e) => outputList.Add(e.Data);
            (int code, string[] errors) = await runner.RunThrottled(process, urls, options, ct, progress, output);
            return new RunResult<string[]>(code == 0, errors, outputList.ToArray());
        }

        /// <summary>
        /// Runs youtube-dl with the given option set and additional parameters.
        /// </summary>
        /// <param name="url">The video URL passed to youtube-dl.</param>
        /// <param name="options">The OptionSet of youtube-dl options.</param>
        /// <param name="ct">A CancellationToken used to cancel the process.</param>
        /// <param name="progress">A progress provider used to get download progress information.</param>
        /// <param name="output">A progress provider used to capture the standard output.</param>
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
        /// Runs an update of youtube-dl.
        /// </summary>
        /// <returns>The output of youtube-dl as string.</returns>
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
        /// <param name="overrideOptions">Override options of the default option set for this run.</param>
        /// <param name="progress">A progress provider used to get download progress information.</param>
        /// <param name="output">A progress provider used to capture the standard output.</param>
        /// <param name="useFfmpegMetaDataFallback">If set to true, Ffmpeg will be used to fetch missing metadata values</param>
        /// <returns>A RunResult object containing a VideoData object with the requested video information.</returns>
        public async Task<RunResult<VideoData>> RunVideoDataFetch(string url,
            CancellationToken ct = default, bool flat = true, OptionSet overrideOptions = null,
            IProgress<DownloadProgress> progress = null, IProgress<string> output = null, bool useFfmpegMetaDataFallback = false)
        {
            var opts = GetDownloadOptions();
            opts.DumpSingleJson = true;
            opts.FlatPlaylist = flat;
            if (overrideOptions != null)
            {
                opts = opts.OverrideOptions(overrideOptions);
            }
            VideoData videoData = null;
            var process = new YoutubeDLProcess(YoutubeDLPath);
            process.OutputReceived += (o, e) => videoData = JsonConvert.DeserializeObject<VideoData>(e.Data);
            (int code, string[] errors) = await runner.RunThrottled(process, new[] { url }, opts, ct, progress, output);
            if (code == 0 && useFfmpegMetaDataFallback)
            {
                if (videoData.Formats == null && videoData.Entries != null)
                {
                    List<FormatData> entryFormats = new List<FormatData>();
                    for(int i = videoData.Entries.Length - 1; i >= 0; i--)
                    {
                        VideoData child = videoData.Entries[i];
                        if (child.Formats == null)
                        {
                            entryFormats.Add(new FormatData()
                            {
                                Url = child.Url,
                                Format = child.Format,
                                Extension = child.Extension
                            });
                        }
                        else
                        {
                            entryFormats.AddRange(videoData.Entries[i].Formats);
                        }
                    }

                    var processed = await executeFallback(entryFormats.Distinct().ToArray(), output);
                    videoData.Formats = processed.ToArray();
                    if (videoData.Duration == null)
                    {
                        var first = videoData.Formats.First(f => f.Duration != null);
                        if (first != null)
                            videoData.Duration = first.Duration;
                    }
                }
                else if(videoData.Formats != null && videoData.Formats.Length > 0)
                {
                    videoData.Formats = await executeFallback(videoData.Formats, output);
                    if (videoData.Duration == null) {
                        var first = videoData.Formats.First(f => f.Duration != null);
                        if (first != null)
                            videoData.Duration = first.Duration;
                    }
                }

            }
            return new RunResult<VideoData>(code == 0, errors, videoData);
        }

        private async Task<FormatData[]> executeFallback(FormatData[] formats, IProgress<string> output = null)
        {
            if (formats != null && formats.Length > 0 && formats.Where(f => f.VideoCodec == null && f.AudioCodec == null).Count() > 0)
            {
                if (string.IsNullOrEmpty(FFmpeg.ExecutablesPath))
                    throw new Exception("Ffmpeg Executables Path Not Defined");
                
                    int item = 0;
                    output?.Report($"MetaData Incomplete - Analyzing [{item}/{formats.Length}]");
                foreach (var format in formats)
                {
                    item++;
                    output?.Report($"MetaData Incomplete - Analyzing [{item}/{formats.Length}]");
                    if ((format.VideoCodec == null || format.VideoCodec == "none") && (format.AudioCodec == null || format.AudioCodec == "none"))
                    {
                        float? dur = null;
                        var fInfo = await FFmpeg.GetMediaInfo(format.Url);
                        if (fInfo != null)
                        {
                            if (fInfo.Duration != null)
                                dur = (float?)fInfo.Duration.TotalSeconds;
                            format.FileSize = (long?)fInfo.Size;
                            if (fInfo.VideoStreams != null && fInfo.VideoStreams.Count() > 0)
                            {
                                var vid = fInfo.VideoStreams.First();
                                if (vid != null)
                                {
                                    format.Bitrate = (double?)vid.Bitrate;
                                    format.VideoCodec = vid.Codec;
                                    format.Width = (int?)vid.Width;
                                    format.Height = (int?)vid.Height;
                                    format.VideoBitrate = (double?)vid.Bitrate;
                                    format.FrameRate = (float?)vid.Framerate;
                                }
                            }
                            if (fInfo.AudioStreams != null && fInfo.AudioStreams.Count() > 0)
                            {
                                var aud = fInfo.AudioStreams.First();
                                if (aud != null)
                                {
                                    if (dur == null && aud.Duration != null)
                                        dur = (float?)aud.Duration.TotalSeconds;
                                    format.AudioCodec = aud.Codec;
                                    format.AudioBitrate = (double?)aud.Bitrate;
                                    format.AudioSamplingRate = (double?)aud.SampleRate;
                                }
                            }
                            else
                            {
                                format.AudioCodec = "none";
                            }
                        }
                        if (format.Duration == null)
                            format.Duration = dur;
                    }
                }                
            }
            return formats;
        }

        /// <summary>
        /// Runs a download of the specified video with an optional conversion afterwards.
        /// </summary>
        /// <param name="url">The URL of the video to be downloaded.</param>
        /// <param name="format">A format selection string in youtube-dl style.</param>
        /// <param name="mergeFormat">If a merge is required, the container format of the merged downloads.</param>
        /// <param name="recodeFormat">The video format the output will be recoded to after download.</param>
        /// <param name="ct">A CancellationToken used to cancel the download.</param>
        /// <param name="progress">A progress provider used to get download progress information.</param>
        /// <param name="output">A progress provider used to capture the standard output.</param>
        /// <param name="overrideOptions">Override options of the default option set for this run.</param>
        /// <param name="outputArgs">Passes full argument list to output when true</param>
        /// <returns>A RunResult object containing the path to the downloaded and converted video.</returns>
        public async Task<RunResult<string>> RunVideoDownload(string url,
            string format = "bestvideo+bestaudio/best",
            DownloadMergeFormat mergeFormat = DownloadMergeFormat.Unspecified,
            VideoRecodeFormat recodeFormat = VideoRecodeFormat.None,
            CancellationToken ct = default, IProgress<DownloadProgress> progress = null,
            IProgress<string> output = null, OptionSet overrideOptions = null, bool outputArgs = true)
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
            if (outputArgs)
                output?.Report($"Arguments: {process.ConvertToArgs(new[] { url }, opts)}\n");
            else
                output?.Report($"Starting Download: {url}");
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
        /// <param name="format">A format selection string in youtube-dl style.</param>
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
            IProgress<string> output = null, OptionSet overrideOptions = null, bool outputArgs = true)
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
            if (outputArgs)
                output?.Report($"Arguments: {process.ConvertToArgs(new[] { url }, opts)}\n");
            else
                output?.Report($"Starting Download: {url}");
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
        public async Task<RunResult<string>> RunAudioDownload(string url, AudioConversionFormat format,
            CancellationToken ct = default, IProgress<DownloadProgress> progress = null,
            IProgress<string> output = null, OptionSet overrideOptions = null, bool outputArgs = true)
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
            if (outputArgs)
                output?.Report($"Arguments: {process.ConvertToArgs(new[] { url }, opts)}\n");
            else
                output?.Report($"Starting Download: {url}");
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
            IProgress<string> output = null, OptionSet overrideOptions = null, bool outputArgs = true)
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
            if (outputArgs)
                output?.Report($"Arguments: {process.ConvertToArgs(new[] { url }, opts)}\n");
            else
                output?.Report($"Starting Download: {url}");
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
                HlsPreferNative = true,
                ExternalDownloaderArgs = "ffmpeg:-nostats -loglevel 0",
                Output = Path.Combine(OutputFolder, OutputFileTemplate),
                RestrictFilenames = this.RestrictFilenames,
                NoContinue = this.OverwriteFiles,
                NoOverwrites = !this.OverwriteFiles,
                NoPart = true,
                FfmpegLocation = Utils.GetFullPath(this.FFmpegPath),
                Exec = "echo {}"
            };
        }

        #endregion
    }
}
