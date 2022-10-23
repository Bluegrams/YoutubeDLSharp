using System.Collections.Generic;

namespace YoutubeDLSharp.Options
{
    /// <summary>
    /// Interface for one yt-dlp option.
    /// </summary>
    public interface IOption
    {
        /// <summary>
        /// The default string representation of the option flag.
        /// </summary>
        string DefaultOptionString { get; }
        /// <summary>
        /// An array of all possible string representations of the option flag.
        /// </summary>
        string[] OptionStrings { get; }
        /// <summary>
        /// True if the option flag is set; false otherwise.
        /// </summary>
        bool IsSet { get; }
        /// <summary>
        /// Sets the option value from a given string representation.
        /// </summary>
        /// <param name="s">The string (including the option flag).</param>
        void SetFromString(string s);
        /// <summary>
        /// Converts the option to a collection of string representations.
        /// This is relevant for MultiOption instances that can have multiple values.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> ToStringCollection();

        /// <summary>
        /// True if this option is custom.
        /// </summary>
        bool IsCustom { get; }
    }
}
