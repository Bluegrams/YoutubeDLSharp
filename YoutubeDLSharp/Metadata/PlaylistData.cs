using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YoutubeDLSharp.Metadata
{
    public class PlaylistData
    {
        [JsonProperty("uploader")]
        public string Uploader { get; set; }

        [JsonProperty("uploader_id")]
        public string UploaderId { get; set; }

        [JsonProperty("uploader_url")]
        public string UploaderUrl { get; set; }

        [JsonProperty("thumbnails")]
        public List<ThumbnailData> Thumbnails { get; set; }

        [JsonProperty("tags")]
        public List<object> Tags { get; set; }

        [JsonProperty("view_count")]
        public int? ViewCount { get; set; }

        [JsonProperty("availability")]
        public string Availability { get; set; }

        [JsonProperty("modified_date")]
        public string ModifiedDate { get; set; }

        [JsonProperty("playlist_count")]
        public int? PlaylistCount { get; set; }

        [JsonProperty("channel_follower_count")]
        public object ChannelFollowerCount { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("channel_url")]
        public string ChannelUrl { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("_type")]
        public string Type { get; set; }

        [JsonProperty("entries")]
        public List<VideoData> Entries { get; set; }

        [JsonProperty("webpage_url")]
        public string WebpageUrl { get; set; }

        [JsonProperty("original_url")]
        public string OriginalUrl { get; set; }

        [JsonProperty("webpage_url_basename")]
        public string WebpageUrlBasename { get; set; }

        [JsonProperty("webpage_url_domain")]
        public string WebpageUrlDomain { get; set; }

        [JsonProperty("extractor")]
        public string Extractor { get; set; }

        [JsonProperty("extractor_key")]
        public string ExtractorKey { get; set; }

        [JsonProperty("requested_entries")]
        public List<int> RequestedEntries { get; set; }

        [JsonProperty("epoch")]
        public int? Epoch { get; set; }
    }
}
