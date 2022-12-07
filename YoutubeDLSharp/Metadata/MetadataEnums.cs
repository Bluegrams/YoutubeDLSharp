
using System.Runtime.Serialization;

namespace YoutubeDLSharp.Metadata
{
    //https://github.com/yt-dlp/yt-dlp/blob/9c53b9a1b6b8914e4322263c97c26999f2e5832e/yt_dlp/extractor/common.py#L105-L403

    /// <summary>
    /// Possible types of media fetched by yt-dlp.
    /// </summary>
    public enum MetadataType
    {
        [EnumMember(Value = "video")]
        Video,
        [EnumMember(Value = "playlist")]
        Playlist,
        [EnumMember(Value = "multi_video")]
        MultiVideo,
        [EnumMember(Value = "url")]
        Url,
        [EnumMember(Value = "url_transparent")]
        UrlTransparent
    }

    public enum LiveStatus
    {
        [EnumMember(Value = "unknown")]
        None,
        [EnumMember(Value = "is_live")]
        IsLive,
        [EnumMember(Value = "is_upcoming")]
        IsUpcoming,
        [EnumMember(Value = "was_live")]
        WasLive,
        [EnumMember(Value = "not_live")]
        NotLive,
        [EnumMember(Value = "post_live")]
        PostLive
    }

    public enum Availability
    {
        [EnumMember(Value = "private")]
        Private,
        [EnumMember(Value = "premium_only")]
        PremiumOnly,
        [EnumMember(Value = "subscriber_only")]
        SubscriberOnly,
        [EnumMember(Value = "needs_auth")]
        NeedsAuth,
        [EnumMember(Value = "unlisted")]
        Unlisted,
        [EnumMember(Value = "public")]
        Public
    }
}
