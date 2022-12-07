using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;

namespace YoutubeDLSharp.Converters
{
    public class DateTimeConverter : JsonConverter<DateTime> //switch to DateOOnly when fully moved to .Net 6
    {
        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var data = DateTime.ParseExact(reader.Value.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
            return data;           
        }

        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }

    public class UnixTimestampConverter : JsonConverter<DateTime>
    {
        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.ReadAsDateTime().Value.ToUniversalTime();
        }

        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
