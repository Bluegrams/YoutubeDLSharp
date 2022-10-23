using System;
using System.ComponentModel;
using System.Globalization;

namespace YoutubeDLSharp.Options
{
    internal static class Utils
    {
        internal static T OptionValueFromString<T>(string stringValue)
        {
            if (typeof(T) == typeof(bool))
            {
                return (T)(object)true;
            }
            else if (typeof(T) == typeof(Enum))
            {
                string titleCase = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(stringValue);
                return (T)Enum.Parse(typeof(T), titleCase);
            }
            else if (typeof(T) == typeof(DateTime))
            {
                return (T)(object)DateTime.ParseExact(stringValue, "yyyyMMdd", null);
            }
            else
            {
                TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
                return (T)conv.ConvertFrom(stringValue);
            }
        }

        internal static string OptionValueToString<T>(T value)
        {
            string val;
            if (value is bool)
                val = String.Empty;
            else if (value is Enum)
                val = $" \"{value.ToString().ToLower()}\"";
            else if (value is DateTime dateTime)
                val = $" {dateTime.ToString("yyyyMMdd")}";
            else if (value is string)
                val = $" \"{value}\"";
            else val = " " + value;
            return val;
        }
    }
}
