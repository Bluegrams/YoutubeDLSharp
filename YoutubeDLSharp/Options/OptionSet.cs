using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            foreach (var opt in GetOptions())
            {
                var value = opt.ToString();
                if (!String.IsNullOrWhiteSpace(value))
                    yield return value;
            }
        }

        internal IEnumerable<IOption> GetOptions()
        {
            return this.GetType().GetRuntimeFields()
                .Where(p => p.FieldType.IsGenericType && p.FieldType.GetGenericTypeDefinition() == typeof(Option<>))
                .Select(p => p.GetValue(this)).Cast<IOption>();
        }

        /// <summary>
        /// Creates an option set from an array of command-line option strings.
        /// </summary>
        /// <param name="lines">An array containing one command-line option string per item.</param>
        /// <returns>The parsed OptionSet.</returns>
        public static OptionSet FromString(string[] lines)
        {
            OptionSet optSet = new OptionSet();
            var options = optSet.GetOptions();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                // skip comments
                if (line.StartsWith("#"))
                    continue;
                string flag = line.Split(' ')[0];
                IOption option = options.Where(o => o.OptionStrings.Contains(flag))
                                        .FirstOrDefault();
                if (option != null)
                {
                    option.SetFromString(line);
                }
                else throw new FormatException($"Invalid option in line {i+1}: {line}");
            }
            return optSet;
        }

        /// <summary>
        /// Loads an option set from a youtube-dl config file.
        /// </summary>
        /// <param name="path">The path to the config file.</param>
        /// <returns>The loaded OptionSet.</returns>
        public static OptionSet LoadConfigFile(string path)
        {
            return FromString(File.ReadAllLines(path));
        }
    }
}
