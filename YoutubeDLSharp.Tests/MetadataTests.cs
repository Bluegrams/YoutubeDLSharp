using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YoutubeDLSharp.Metadata;

namespace YoutubeDLSharp.Tests
{
    [TestClass]
    public class MetadataTests
    {
        private static YoutubeDL ydl;

        [ClassInitialize]
        public static async Task Initialize(TestContext context)
        {
            await PrepTests.DownloadBinaries();
            ydl = new YoutubeDL();
            ydl.OutputFileTemplate = "%(title)s.%(ext)s";
            Trace.WriteLine("yt-dlp Version: " + ydl.Version);
        }

        [TestMethod]
        public async Task TestVideoInformationYoutube()
        {
            string url = "https://www.youtube.com/watch?v=C0DPdy98e4c&t=9s";
            RunResult<VideoData> result = await ydl.RunVideoDataFetch(url);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(MetadataType.Video, result.Data.ResultType);
            Assert.AreEqual("TEST VIDEO", result.Data.Title);
            Assert.AreEqual("Youtube", result.Data.ExtractorKey);
            Assert.AreEqual(new DateTime(2007, 02, 21), result.Data.UploadDate);
            Assert.IsNotNull(result.Data.Formats);
            Assert.IsNotNull(result.Data.Tags);
            Assert.IsNull(result.Data.Entries);
        }

        [TestMethod]
        public async Task TestVideoInformationYoutube_Premier()
        {
            string url = "https://youtu.be/HEdbxIfAKyk";
            RunResult<VideoData> result = await ydl.RunVideoDataFetch(url);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(MetadataType.Video, result.Data.ResultType);
            Assert.AreEqual("TEST VIDEO PREMIER", result.Data.Title);
            Assert.AreEqual("Youtube", result.Data.ExtractorKey);
            Assert.AreEqual(new DateTime(2020, 11, 25), result.Data.UploadDate);
            Assert.IsNotNull(result.Data.Formats);
            Assert.IsNotNull(result.Data.Tags);
            Assert.IsNull(result.Data.Entries);
        }
        
        [TestMethod]
        public async Task TestPlaylistInformation()
        {
            string url = "https://www.youtube.com/playlist?list=PLD8804CB40CAB0EA5";
            RunResult<VideoData> result = await ydl.RunVideoDataFetch(url);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(MetadataType.Playlist, result.Data.ResultType);
            Assert.AreEqual("E.Grieg Peer Gynt Suite playlist", result.Data.Title);
            Assert.IsNotNull(result.Data.Entries);
        }
    }
}
