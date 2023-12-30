using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YoutubeDLSharp.Options;

namespace YoutubeDLSharp.Tests
{
    [TestClass]
    public class OptionTests
    {
        [TestMethod]
        public void TestBooleanOptions()
        {
            var expected = new[]
            {
                "--ignore-config", "--no-playlist", "--no-continue", "-q", "--embed-thumbnail"
            };
            var options = new OptionSet()
            {
                IgnoreConfig = true,
                NoPlaylist = true,
                NoContinue = true,
                Quiet = true,
                EmbedThumbnail = true
            };
            CollectionAssert.AreEquivalent(expected, options.GetOptionFlags().ToArray());
        }

        [TestMethod]
        public void TestEnumOptions()
        {
            var expected = new[]
            {
                "-x", "--audio-format \"mp3\""
            };
            var options = new OptionSet()
            {
                ExtractAudio = true,
                AudioFormat = AudioConversionFormat.Mp3
            };
            CollectionAssert.AreEquivalent(expected, options.GetOptionFlags().ToArray());
        }

        [TestMethod]
        public void TestStringOptions()
        {
            var expected = new[]
            {
                "-f \"mp4/bestvideo\"", "-R 10", "--file-access-retries 100", "--cookies-from-browser \"firefox\""
            };
            var options = new OptionSet()
            {
                Format = "mp4/bestvideo",
                Retries = 10,
                FileAccessRetries = 100,
                CookiesFromBrowser = "firefox"
            };
            CollectionAssert.AreEquivalent(expected, options.GetOptionFlags().ToArray());
        }

        [TestMethod]
        public void TestDateTimeOptions()
        {
            var expected = new[]
            {
                "--dateafter 20190101", "--datebefore 20191020"
            };
            var options = new OptionSet()
            {
                DateAfter = new DateTime(2019, 01, 01),
                DateBefore = new DateTime(2019, 10, 20)
            };
            CollectionAssert.AreEquivalent(expected, options.GetOptionFlags().ToArray());
        }

        [TestMethod]
        public void TestMultiOption()
        {
            var expected = new[]
            {
                "--exec \"echo {}\"", "--exec \"pwd\""
            };
            var options = new OptionSet()
            {
                Exec = new[]
                {
                    "echo {}",
                    "pwd"
                }
            };
            var actual = options.GetOptionFlags().ToArray();
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
