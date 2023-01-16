using Newtonsoft.Json;
using System;
using YoutubeDLSharp.Converters;

namespace YoutubeDLSharp.Metadata
{
    //https://github.com/yt-dlp/yt-dlp/blob/9c53b9a1b6b8914e4322263c97c26999f2e5832e/yt_dlp/extractor/common.py#L105-L403
    public class CommentData
    {
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("author_id")]
        public string AuthorID { get; set; }
        [JsonProperty("author_thumbnail")]
        public string AuthorThumbnail { get; set; }
        [JsonProperty("html")]
        public string Html { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("timestamp")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime Timestamp { get; set; } //UNIX Timestamp
        [JsonProperty("parent")]
        public string Parent { get; set; }
        [JsonProperty("like_count")]
        public int? LikeCount { get; set; }
        [JsonProperty("dislike_count")]
        public int? DislikeCount { get; set; }
        [JsonProperty("is_favorited")]
        public bool? IsFavorited { get; set; }
        [JsonProperty("author_is_uploader")]
        public bool? AuthorIsUploader { get; set; }

        //Unused Fields (These are fields that were excluded, but documented for future use:
        //time_text  
    }
}
