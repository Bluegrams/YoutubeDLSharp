using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YoutubeDLSharp.Metadata;

namespace YoutubeDLSharp.Tests
{
    [TestClass]
    public class MetadataTests
    {
        private static YoutubeDl _ydl;

        [ClassInitialize]
        public static async Task Initialize(TestContext context)
        {
            await PrepTests.DownloadBinaries();
            _ydl = new YoutubeDl
            {
                YoutubeDlPath = "yt-dlp",
                FFmpegPath = "ffmpeg"
            };
        }

        [TestMethod]
        public async Task TestVideoInformationYoutube()
        {
            const string url = "https://www.youtube.com/watch?v=C0DPdy98e4c&t=9s";
            var result = await _ydl.RunVideoDataFetch(url);
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
        public async Task TestVideoInformationVimeo()
        {
            const string url = "https://vimeo.com/23608259";
            var result = await _ydl.RunVideoDataFetch(url);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(MetadataType.Video, result.Data.ResultType);
            Assert.AreEqual("Cats in Tanks", result.Data.Title);
            Assert.AreEqual("Vimeo", result.Data.ExtractorKey);
            Assert.AreEqual("Whitehouse Post", result.Data.Uploader);
            Assert.IsNotNull(result.Data.Formats);
            Assert.IsNull(result.Data.Entries);
        }

        [TestMethod]
        public async Task TestPlaylistInformation()
        {
            const string url = "https://www.youtube.com/playlist?list=PLD8804CB40CAB0EA5";
            var result = await _ydl.RunVideoDataFetch(url);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(MetadataType.Playlist, result.Data.ResultType);
            Assert.AreEqual("E.Grieg Peer Gynt Suite playlist", result.Data.Title);
            Assert.IsNotNull(result.Data.Entries);
        }
    }
}
