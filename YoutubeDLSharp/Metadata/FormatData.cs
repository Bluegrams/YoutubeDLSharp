using Newtonsoft.Json;
using YoutubeDLSharp.Converters;

namespace YoutubeDLSharp.Metadata
{
    // https://github.com/yt-dlp/yt-dlp/blob/85b33f5c163f60dbd089a6b9bc2ba1366d3ddf93/yt_dlp/extractor/common.py#L105-L534

    /// <summary>
    /// Represents information for one available download format for one video as extracted by yt-dlp.
    /// </summary>
    public class FormatData
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("manifest_url")]
        public string ManifestUrl { get; set; }
        [JsonProperty("ext")]
        public string Extension { get; set; }
        [JsonProperty("format")]
        public string Format { get; set; }
        [JsonProperty("format_id")]
        public string FormatId { get; set; }
        [JsonProperty("format_note")]
        public string FormatNote { get; set; }
        [JsonProperty("width")]
        public int? Width { get; set; }
        [JsonProperty("height")]
        public int? Height { get; set; }
        [JsonProperty("resolution")]
        public string Resolution { get; set; }
        [JsonProperty("dynamic_range")]
        public string DynamicRange { get; set; }
        [JsonProperty("tbr")]
        public double? Bitrate { get; set; }
        [JsonProperty("abr")]
        public double? AudioBitrate { get; set; }
        [JsonProperty("acodec")]
        public string AudioCodec { get; set; }
        [JsonProperty("asr")]
        public double? AudioSamplingRate { get; set; }
        [JsonProperty("audio_channels")]
        public int? AudioChannels { get; set; }
        [JsonProperty("vbr")]
        public double? VideoBitrate { get; set; }
        [JsonProperty("fps")]
        public float? FrameRate { get; set; }
        [JsonProperty("vcodec")]
        public string VideoCodec { get; set; }
        [JsonProperty("container")]
        public string ContainerFormat { get; set; }
        [JsonProperty("filesize")]
        public long? FileSize { get; set; }
        [JsonProperty("filesize_approx")]
        public long? ApproximateFileSize { get; set; }
        [JsonProperty("player_url")]
        public string PlayerUrl { get; set; }
        [JsonProperty("protocol")]
        public string Protocol { get; set; }
        [JsonProperty("fragment_base_url")]
        public string FragmentBaseUrl { get; set; }
        [JsonProperty("is_from_start")]
        public bool? IsFromStart { get; set; }
        [JsonProperty("preference")]
        public int? Preference { get; set; }
        [JsonProperty("language")]
        public string Language { get; set; }
        [JsonProperty("language_preference")]
        public int? LanguagePreference { get; set; }
        [JsonProperty("quality")]
        public double? Quality { get; set; }
        [JsonProperty("source_preference")]
        public int? SourcePreference { get; set; }
        [JsonProperty("stretched_ratio")]
        public float? StretchedRatio { get; set; }
        [JsonProperty("no_resume")]
        public bool? NoResume { get; set; }
        [JsonConverter(typeof(StringToEnumConverter<MaybeBool>))]
        [JsonProperty("has_drm")]
        public MaybeBool HasDRM { get; set; }

        public override string ToString() => $"[{Extension}] {Format}";

        //Unused Fields (These are fields that were excluded, but documented for future use:
        //downloader_options (internal use only)        
        //fragments        
        //http_headers                
        //manifest_stream_number (internal use only)                        
    }
}
