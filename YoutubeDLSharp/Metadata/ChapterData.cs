using Newtonsoft.Json;

namespace YoutubeDLSharp.Metadata
{
    public class ChapterData
    {
        [JsonProperty("start_time")]
        public float? StartTime { get; set; }
        [JsonProperty("end_time")]
        public float? EndTime { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
