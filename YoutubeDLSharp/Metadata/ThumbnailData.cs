using System.Text.Json.Serialization;

namespace YoutubeDLSharp.Metadata;

public class ThumbnailData
{
    [JsonPropertyName("id")]
    public string ID { get; set; }
    [JsonPropertyName("url")]
    public string Url { get; set; }
    [JsonPropertyName("preference")]
    public int? Preference { get; set; }
    [JsonPropertyName("width")]
    public int? Width { get; set; }
    [JsonPropertyName("height")]
    public int? Height { get; set; }
    [JsonPropertyName("resolution")]
    public string Resolution { get; set; }
    [JsonPropertyName("filesize")]
    public int? Filesize { get; set; }
}