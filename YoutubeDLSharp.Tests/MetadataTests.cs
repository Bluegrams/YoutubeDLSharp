using System;
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
        public static void Initialize(TestContext context)
        {
            ydl = new YoutubeDL();
            ydl.YoutubeDLPath = "Lib\\youtube-dl.exe";
            ydl.FFmpegPath = "Lib\\ffmpeg.exe";
        }

        [TestMethod]
        public async Task TestVideoInformationYoutube()
        {
            string url = "https://www.youtube.com/watch?v=_QdPW8JrYzQ";
            RunResult<VideoData> result = await ydl.RunVideoDataFetch(url);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(MetadataType.Video, result.Data.ResultType);
            Assert.AreEqual("This is what happens when you reply to spam email | James Veitch", result.Data.Title);
            Assert.AreEqual("Youtube", result.Data.ExtractorKey);
            Assert.AreEqual(new DateTime(2016, 02, 01), result.Data.UploadDate);
            Assert.IsNotNull(result.Data.Formats);
            Assert.IsNotNull(result.Data.Tags);
            Assert.IsNull(result.Data.Entries);
        }

        [TestMethod]
        public async Task TestVideoInformationVimeo()
        {
            string url = "https://vimeo.com/23608259";
            RunResult<VideoData> result = await ydl.RunVideoDataFetch(url);
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
            string url = "https://www.youtube.com/playlist?list=PLD8804CB40CAB0EA5";
            RunResult<VideoData> result = await ydl.RunVideoDataFetch(url);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(MetadataType.Playlist, result.Data.ResultType);
            Assert.AreEqual("E.Grieg Peer Gynt Suite playlist", result.Data.Title);
            Assert.AreEqual("YoutubePlaylist", result.Data.ExtractorKey);
            Assert.IsNotNull(result.Data.Entries);
        }
    }
}
