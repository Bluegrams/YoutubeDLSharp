using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace YoutubeDLSharp.Converters
{    
    public class UnixTimestampConverter : JsonConverter<DateTime?>
    {
        public override DateTime? ReadJson(JsonReader reader, Type objectType, DateTime? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if(reader.Value == null)
            {
                return null;
            }
            else
            {
                var value = (long)reader.Value;
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var timeSpan = TimeSpan.FromSeconds(value);
                var utc = epoch.Add(timeSpan).ToUniversalTime();
                return utc;
            }
        }

        public override void WriteJson(JsonWriter writer, DateTime? value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }

    public class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            DateTimeFormat = "yyyyMMdd";
        }
    }
}
