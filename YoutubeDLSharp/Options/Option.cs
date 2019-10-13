using System;

namespace YoutubeDLSharp.Options
{
    /// <summary>
    /// Represents one youtube-dl option.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    public class Option<T>
    {
        private T value;

        /// <summary>
        /// The string representation of the option flag.
        /// </summary>
        public string OptionString { get; }

        /// <summary>
        /// True if the option flag is set; false otherwise.
        /// </summary>
        public bool IsSet { get; private set; }

        /// <summary>
        /// The option value.
        /// </summary>
        public T Value
        {
            get => value;
            set
            {
                
                this.IsSet = !object.Equals(value, default(T));
                this.value = value;
            }
        }

        /// <summary>
        /// Creates a new instance of class Option.
        /// </summary>
        public Option(params string[] optionStrings)
        {
            OptionString = optionStrings[0];
            IsSet = false;
        }

        public override string ToString()
        {
            if (!IsSet) return String.Empty;
            string val;
            if (Value is bool)
                val = String.Empty;
            else if (Value is Enum)
                val = $" \"{Value.ToString().ToLower()}\"";
            else if (Value is DateTime dateTime)
                val = $" {dateTime.ToString("yyyyMMdd")}";
            else if (Value is string)
                val = $" \"{Value}\"";
            else val = " " + Value;
            return OptionString + val;
        }
    }
}
