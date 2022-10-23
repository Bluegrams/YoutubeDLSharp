using Newtonsoft.Json;

namespace YoutubeDLSharp.Metadata
{
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
        public long? Timestamp { get; set; }
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
    }
}
