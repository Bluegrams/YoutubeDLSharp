using Newtonsoft.Json;

namespace YoutubeDLSharp.Metadata
{
    //https://github.com/yt-dlp/yt-dlp/blob/9c53b9a1b6b8914e4322263c97c26999f2e5832e/yt_dlp/extractor/common.py#L105-L403
    public class ThumbnailData
    {
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("preference")]
        public int? Preference { get; set; }
        [JsonProperty("width")]
        public int? Width { get; set; }
        [JsonProperty("height")]
        public int? Height { get; set; }
        [JsonProperty("resolution")]
        public string Resolution { get; set; }
        [JsonProperty("filesize")]
        public int? Filesize { get; set; }

        //Unused Fields (These are fields that were excluded, but documented for future use:
        //http_headers
    }
}
