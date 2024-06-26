﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using YoutubeDLSharp.Metadata;

namespace YoutubeDLSharp.Converters
{    
    public class StringToEnumConverter<T> : JsonConverter<T> where T : Enum
    {
        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer) 
        {
            var value = reader.Value?.ToString();
            if(value == null)
            {
                return default(T);
            }
            var jsonString = $"'{value.ToLower()}'";
            var enumValue = JsonConvert.DeserializeObject<T>(jsonString, new StringEnumConverter());
            return enumValue;
        }

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }

    public class StringToNullableIntConverter : JsonConverter<int?>
    {
        public override int? ReadJson(JsonReader reader, Type objectType, int? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = reader.Value?.ToString();
            if (value == null) return default;
            else if (value.Contains(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator))
            {
                value = value.Split(Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator))[0];
            }
            if (int.TryParse(value, out var intValue))
            {
                return intValue;
            }
            return default;
        }

        public override void WriteJson(JsonWriter writer, int? value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}
