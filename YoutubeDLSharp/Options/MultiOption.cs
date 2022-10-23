using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YoutubeDLSharp.Options
{
    /// <summary>
    /// Represents a yt-dlp option that can be set multiple times.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    public class MultiOption<T> : IOption
    {
        private MultiValue<T> value;

        public string DefaultOptionString => OptionStrings.Last();

        public string[] OptionStrings { get; }

        public bool IsSet { get; private set; }

        public bool IsCustom { get; }

        public MultiValue<T> Value
        {
            get => value;
            set
            {
                this.IsSet = !object.Equals(value, default(T));
                this.value = value;
            }
        }

        public MultiOption(params string[] optionStrings)
        {
            OptionStrings = optionStrings;
            IsSet = false;
        }

        public MultiOption(bool isCustom, params string[] optionStrings)
        {
            OptionStrings = optionStrings;
            IsSet = false;
            IsCustom = isCustom;
        }

        public void SetFromString(string s)
        {
            string[] split = s.Split(' ');
            string stringValue = s.Substring(split[0].Length).Trim().Trim('"');
            if (!OptionStrings.Contains(split[0]))
                throw new ArgumentException("Given string does not match required format.");
            // Set as initial value or append to existing
            T newValue = Utils.OptionValueFromString<T>(stringValue);
            if (!IsSet)
            {
                Value = newValue;
            }
            else
            {
                Value.Values.Add(newValue);
            }
        }

        public override string ToString() => String.Join(" ", ToStringCollection());

        public IEnumerable<string> ToStringCollection()
        {
            if (!IsSet) return new[] {""};
            List<string> strings = new List<string>();
            foreach (T value in Value.Values)
            {
                strings.Add(DefaultOptionString + Utils.OptionValueToString(value));
            }
            return strings;
        }
    }
}
