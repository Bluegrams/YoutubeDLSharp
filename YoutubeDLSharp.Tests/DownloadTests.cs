using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YoutubeDLSharp.Options;

namespace YoutubeDLSharp.Tests;

[TestClass]
public class DownloadTests
{
    private const string Url = "https://www.youtube.com/watch?v=C0DPdy98e4c";
    private static YoutubeDl _ydl;
    private static List<string> _downloadedFiles;

    [ClassInitialize]
    public static async Task Initialize(TestContext context)
    {
        await PrepTests.DownloadBinaries();
        _ydl = new YoutubeDl
        {
            YoutubeDlPath = "yt-dlp",
            FFmpegPath = "ffmpeg"
        };
        _downloadedFiles = new List<string>();
    }

    [TestMethod]
    public async Task TestVideoDownloadSimple()
    {
        var result = await _ydl.RunVideoDownload(Url, mergeFormat: DownloadMergeFormat.Mkv);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(string.Empty, string.Join("", result.ErrorOutput));
        var file = result.Data;
        Assert.IsTrue(File.Exists(file));
        Assert.AreEqual(".mkv", Path.GetExtension(file));
        Assert.AreEqual("TEST VIDEO", Path.GetFileNameWithoutExtension(file));
        _downloadedFiles.Add(file);
    }

    [TestMethod]
    public async Task TestAudioDownloadSimple()
    {
        var result = await _ydl.RunAudioDownload(Url, AudioConversionFormat.Mp3);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(string.Empty, string.Join("", result.ErrorOutput));
        var file = result.Data;
        Assert.IsTrue(File.Exists(file));
        Assert.AreEqual(".mp3", Path.GetExtension(file));
        Assert.AreEqual("TEST VIDEO", Path.GetFileNameWithoutExtension(file));
        _downloadedFiles.Add(file);
    }

    [TestMethod]
    public async Task TestVideoDownloadWithOptions()
    {
        _ydl.OutputFolder = "Lib";
        _ydl.OutputFileTemplate = "%(extractor)s_%(title)s_%(upload_date)s.%(ext)s";
        _ydl.RestrictFilenames = true;
        var result = await _ydl.RunVideoDownload(Url, format: "bestvideo", recodeFormat: VideoRecodeFormat.Mp4);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(string.Empty, string.Join("", result.ErrorOutput));
        var file = result.Data;
        Assert.IsTrue(File.Exists(file));
        Assert.IsTrue(Path.GetDirectoryName(file).EndsWith("Lib"));
        Assert.AreEqual(".mp4", Path.GetExtension(file));
        Assert.AreEqual("youtube_TEST_VIDEO_20070221", Path.GetFileNameWithoutExtension(file));
        _downloadedFiles.Add(file);
    }

    [TestMethod]
    public async Task TestVideoDownloadWithOverrideOptions()
    {
        var overrideOptions = new OptionSet()
        {
            Output = "%(extractor)s_%(title)s_%(upload_date)s.%(ext)s",
            RestrictFilenames = true,
            RecodeVideo = VideoRecodeFormat.Mp4
        };
        var result = await _ydl.RunVideoDownload(Url, format: "bestvideo", overrideOptions: overrideOptions);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(string.Empty, string.Join("", result.ErrorOutput));
        var file = result.Data;
        Assert.IsTrue(File.Exists(file));
        Assert.AreEqual(".mp4", Path.GetExtension(file));
        Assert.AreEqual("youtube_TEST_VIDEO_20070221", Path.GetFileNameWithoutExtension(file));
        _downloadedFiles.Add(file);
    }

    [ClassCleanup]
    public static void Cleanup()
    {
        foreach (var file in _downloadedFiles)
        {
            File.Delete(file);
        }
    }
}