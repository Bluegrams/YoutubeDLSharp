using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using YoutubeDLSharp.Metadata;

namespace YoutubeDLSharp.Converters
{
    public class ObjectToStringConverter : JsonConverter<string>
    {
        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return (string)reader.Value;
        }

        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }

    public class AvailabilityJsonConverter : JsonConverter<Availability>
    {
        public override Availability ReadJson(JsonReader reader, Type objectType, Availability existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return (Availability)Enum.Parse(typeof(Availability), (string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, Availability value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }

    public class MetadataJsonConverter : JsonConverter<MetadataType>
    {
        public override MetadataType ReadJson(JsonReader reader, Type objectType, MetadataType existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return (MetadataType)Enum.Parse(typeof(MetadataType), (string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, MetadataType value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }

}
