using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using YoutubeDLSharp.Converters;

namespace YoutubeDLSharp.Metadata
{
    //https://github.com/yt-dlp/yt-dlp/blob/9c53b9a1b6b8914e4322263c97c26999f2e5832e/yt_dlp/extractor/common.py#L105-L403

    /// <summary>
    /// Represents the video metadata for one video as extracted by yt-dlp.
    /// Explanation can be found at https://github.com/yt-dlp/yt-dlp/blob/master/yt_dlp/extractor/common.py#L91.
    /// </summary>
    public class VideoData
    {
        [JsonConverter(typeof(StringToEnumConverter<MetadataType>))]
        [JsonProperty("_type")]
        public MetadataType ResultType { get; set; }
        [JsonProperty("extractor")]
        public string Extractor { get; set; }
        [JsonProperty("extractor_key")]
        public string ExtractorKey { get; set; }
        // If data refers to a playlist:
        [JsonProperty("entries")]
        public VideoData[] Entries { get; set; }
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("formats")]
        public FormatData[] Formats { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("ext")]
        public string Extension { get; set; }
        [JsonProperty("format")]
        public string Format { get; set; }
        [JsonProperty("format_id")]
        public string FormatID { get; set; }
        [JsonProperty("player_url")]
        public string PlayerUrl { get; set; }
        //optional fields
        [JsonProperty("direct")]
        public bool Direct { get; set; }
        [JsonProperty("alt_title")]
        public string AltTitle { get; set; }
        [JsonProperty("display_id")]
        public string DisplayID { get; set; }
        [JsonProperty("thumbnails")]
        public ThumbnailData[] Thumbnails { get; set; }
        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("uploader")]
        public string Uploader { get; set; }
        [JsonProperty("license")]
        public string License { get; set; }
        [JsonProperty("creator")]
        public string Creator { get; set; }
        [JsonConverter(typeof(UnixTimestampConverter))]
        [JsonProperty("release_timestamp")] // date as unix timestamp
        public DateTime? ReleaseTimestamp { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        [JsonProperty("release_date")] // date in UTC (YYYYMMDD).        
        public DateTime? ReleaseDate { get; set; }
        [JsonConverter(typeof(UnixTimestampConverter))]
        [JsonProperty("timestamp")] // date as unix timestamp        
        public DateTime? Timestamp { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        [JsonProperty("upload_date")] // date in UTC (YYYYMMDD).        
        public DateTime? UploadDate { get; set; }
        [JsonConverter(typeof(UnixTimestampConverter))]
        [JsonProperty("modified_timestemp")] // date as unix timestamp
        public DateTime? ModifiedTimestamp { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        [JsonProperty("modified_date")] // date in UTC (YYYYMMDD).        
        public DateTime? ModifiedDate { get; set; }
        [JsonProperty("uploader_id")]
        public string UploaderID { get; set; }
        [JsonProperty("uploader_url")]
        public string UploaderUrl { get; set; }
        [JsonProperty("channel")]
        public string Channel { get; set; }
        [JsonProperty("channel_id")]
        public string ChannelID { get; set; }
        [JsonProperty("channel_url")]
        public string ChannelUrl { get; set; }
        [JsonProperty("channel_follower_count")]
        public long? ChannelFollowerCount { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("subtitles")]
        public Dictionary<string, SubtitleData[]> Subtitles { get; set; }
        [JsonProperty("automatic_captions")]
        public Dictionary<string, SubtitleData[]> AutomaticCaptions { get; set; }
        [JsonProperty("duration")]
        public float? Duration { get; set; }
        [JsonProperty("view_count")]
        public long? ViewCount { get; set; }
        [JsonProperty("concurrent_view_count")]
        public long? ConcurrentViewCount { get; set; }
        [JsonProperty("like_count")]
        public long? LikeCount { get; set; }
        [JsonProperty("dislike_count")]
        public long? DislikeCount { get; set; }
        [JsonProperty("repost_count")]
        public long? RepostCount { get; set; }
        [JsonProperty("average_rating")]
        public double? AverageRating { get; set; }
        [JsonProperty("comment_count")]
        public long? CommentCount { get; set; }
        [JsonProperty("comments")]
        public CommentData[] Comments { get; set; }
        [JsonProperty("age_limit")]
        public int? AgeLimit { get; set; }
        [JsonProperty("webpage_url")]
        public string WebpageUrl { get; set; }
        [JsonProperty("categories")]
        public string[] Categories { get; set; }
        [JsonProperty("tags")]
        public string[] Tags { get; set; }
        [JsonProperty("cast")]
        public string[] Cast { get; set; }
        [JsonProperty("is_live")]
        public bool? IsLive { get; set; }
        [JsonProperty("was_live")]
        public bool? WasLive { get; set; }
        [JsonConverter(typeof(StringToEnumConverter<LiveStatus>))]
        [JsonProperty("live_status")]
        public LiveStatus LiveStatus { get; set; }
        [JsonProperty("start_time")]
        public float? StartTime { get; set; }
        [JsonProperty("end_time")]
        public float? EndTime { get; set; }
        [JsonProperty("playable_in_embed")]
        public string PlayableInEmbed { get; set; }
        [JsonConverter(typeof(StringToEnumConverter<Availability>))]
        [JsonProperty("availability")]
        public Availability? Availability { get; set; }
        [JsonProperty("chapters")]
        public ChapterData[] Chapters { get; set; }
        [JsonProperty("chapter")]
        public string Chapter { get; set; }

        [JsonProperty("chapter_number")]
        public int? ChapterNumber { get; set; }

        [JsonProperty("chapter_id")]
        public string ChapterId { get; set; }

        [JsonProperty("series")]
        public string Series { get; set; }

        [JsonProperty("series_id")]
        public string SeriesId { get; set; }

        [JsonProperty("season")]
        public string Season { get; set; }

        [JsonProperty("season_number")]
        public int? SeasonNumber { get; set; }

        [JsonProperty("season_id")]
        public string SeasonId { get; set; }

        [JsonProperty("episode")]
        public string Episode { get; set; }

        [JsonProperty("episode_number")]
        public int? EpisodeNumber { get; set; }

        [JsonProperty("episode_id")]
        public string EpisodeId { get; set; }

        [JsonProperty("track")]
        public string Track { get; set; }

        [JsonProperty("track_number")]
        public int? TrackNumber { get; set; }

        [JsonProperty("track_id")]
        public string TrackId { get; set; }

        [JsonProperty("artist")]
        public string Artist { get; set; }

        [JsonProperty("genre")]
        public string Genre { get; set; }

        [JsonProperty("album")]
        public string Album { get; set; }

        [JsonProperty("album_type")]
        public string AlbumType { get; set; }

        [JsonProperty("album_artist")]
        public string AlbumArtist { get; set; }

        [JsonProperty("disc_number")]
        public int? DiscNumber { get; set; }

        [JsonProperty("release_year")]
        public string ReleaseYear { get; set; }

        [JsonProperty("composer")]
        public string Composer { get; set; }

        [JsonProperty("section_start")]
        public long? SectionStart { get; set; }

        [JsonProperty("section_end")]
        public long? SectionEnd { get; set; }

        [JsonProperty("rows")]
        public long? StoryboardFragmentRows { get; set; }

        [JsonProperty("columns")]
        public long? StoryboardFragmentColumns { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    
}
