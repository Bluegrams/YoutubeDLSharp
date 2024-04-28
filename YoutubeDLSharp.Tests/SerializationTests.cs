using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using YoutubeDLSharp.Metadata;

namespace YoutubeDLSharp.Tests
{
    [TestClass]
    public class SerializationTests
    {
        [DataTestMethod]
        [DataRow("{'_type': 'multi_video'}", MetadataType.MultiVideo)]
        [DataRow("{'_type': null}", MetadataType.Video)]
        [DataRow("{}", MetadataType.Video)]
        public void TestVideoDataType(string json, MetadataType expected)
        {
            VideoData data = JsonConvert.DeserializeObject<VideoData>(json);
            Assert.AreEqual(expected, data.ResultType);
        }

        [DataTestMethod]
        [DataRow("{'availability': 'needs_auth'}", Availability.NeedsAuth)]
        [DataRow("{'availability': null}", null)]
        [DataRow("{}", null)]
        public void TestVideoDataAvailability(string json, Availability? expected)
        {
            VideoData data = JsonConvert.DeserializeObject<VideoData>(json);
            Assert.AreEqual(expected, data.Availability);
        }

        [DataTestMethod]
        [DataRow("{'width': 640}", 640)]
        [DataRow("{'width': 123.45}", 123)]
        [DataRow("{}", null)]
        public void TestFormatDataNullableInt(string json, int? expected)
        {
            FormatData data = JsonConvert.DeserializeObject<FormatData>(json);
            Assert.AreEqual(expected, data.Width);
        }

        [DataTestMethod]
        [DataRow("{'has_drm': true}", MaybeBool.True)]
        [DataRow("{'has_drm': 'maybe'}", MaybeBool.Maybe)]
        [DataRow("{}", MaybeBool.False)]
        public void TestFormatDataHasDRM(string json, MaybeBool expected)
        {
            FormatData data = JsonConvert.DeserializeObject<FormatData>(json);
            Assert.AreEqual(expected, data.HasDRM);
        }
    }
}
