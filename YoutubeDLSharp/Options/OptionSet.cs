using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace YoutubeDLSharp.Options
{
    /// <summary>
    /// Represents a set of options for youtube-dl.
    /// </summary>
    public partial class OptionSet
    {
        /// <summary>
        /// The default option set (if no options are explicitly set).
        /// </summary>
        public static readonly OptionSet Default = new OptionSet();

        /// <summary>
        /// Writes all options to a config file with the specified path.
        /// </summary>
        public void WriteConfigFile(string path)
        {
            File.WriteAllLines(path, GetOptionFlags());
        }

        public override string ToString() => " " + String.Join(" ", GetOptionFlags());

        /// <summary>
        /// Returns an enumerable of all option flags.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetOptionFlags()
        {
            foreach (var prop in this.GetType().GetRuntimeFields())
            {
                bool isOption = prop.FieldType.IsGenericType
                                && prop.FieldType.GetGenericTypeDefinition() == typeof(Option<>);
                if (isOption)
                {
                    var value = prop.GetValue(this).ToString();
                    if (!String.IsNullOrWhiteSpace(value))
                        yield return value;
                }
            }
        }
    }
}
