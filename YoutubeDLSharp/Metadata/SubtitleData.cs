using System.Text.Json.Serialization;

namespace YoutubeDLSharp.Metadata;

public class SubtitleData
{
    [JsonPropertyName("ext")]
    public string Ext { get; set; }
    [JsonPropertyName("data")]
    public string Data { get; set; }
    [JsonPropertyName("url")]
    public string Url { get; set; }
}