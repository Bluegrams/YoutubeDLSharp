using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace YoutubeDLSharp;

/// <summary>
/// Utility methods.
/// </summary>
public static class Utils
{
    private static readonly Regex RgxTimestamp = new("[0-9]+(?::[0-9]+)+", RegexOptions.Compiled);
    private static readonly Dictionary<char, string> AccentChars
        = "ÂÃÄÀÁÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖŐØŒÙÚÛÜŰÝÞßàáâãäåæçèéêëìíîïðñòóôõöőøœùúûüűýþÿ"
            .Zip(new[] { "A","A","A","A","A","A","AE","C","E","E","E","E","I","I","I","I","D","N",
                    "O","O","O","O","O","O","O","OE","U","U","U","U","U","Y","P","ss",
                    "a","a","a","a","a","a","ae","c","e","e","e","e","i","i","i","i","o","n",
                    "o","o","o","o","o","o","o","oe","u","u","u","u","u","y","p","y"},
                (c, s) => new { Key = c, Val = s }).ToDictionary(o => o.Key, o => o.Val);

    /// <summary>
    /// Sanitize a string to be a valid file name.
    /// Ported from:
    /// https://github.com/ytdl-org/youtube-dl/blob/33c1c7d80fd99024879a5f087b55b24374385e43/youtube_dl/utils.py#L2067
    /// </summary>
    /// <returns></returns>
    public static string Sanitize(string s, bool restricted = false)
    {
        RgxTimestamp.Replace(s, m => m.Groups[0].Value.Replace(':', '_'));
        var result = string.Join("", s.Select(c => SanitizeChar(c, restricted)));
        result = result.Replace("__", "_").Trim('_');
        if (restricted && result.StartsWith("-_"))
            result = result[2..];
        if (result.StartsWith("-"))
            result = "_" + result[1..];
        result = result.TrimStart('.');
        if (string.IsNullOrWhiteSpace(result))
            result = "_";
        return result;
    }

    private static string SanitizeChar(char c, bool restricted)
    {
        if (restricted && AccentChars.ContainsKey(c))
            return AccentChars[c];
        if (c == '?' || c < 32 || c == 127)
            return "";
        switch (c)
        {
            case '"':
                return restricted ? "" : "\'";
            case ':':
                return restricted ? "_-" : " -";
        }

        if ("\\/|*<>".Contains(c))
            return "_";
        switch (restricted)
        {
            case true when "!&\'()[]{}$;`^,# ".Contains(c):
            case true when c > 127:
                return "_";
            default:
                return c.ToString();
        }
    }

    /// <summary>
    /// Returns the absolute path for the specified path string.
    /// Also searches the environment's PATH variable.
    /// </summary>
    /// <param name="fileName">The relative path string.</param>
    /// <returns>The absolute path or null if the file was not found.</returns>
    public static string GetFullPath(string fileName)
    {
        if (File.Exists(fileName))
            return Path.GetFullPath(fileName);

        var values = Environment.GetEnvironmentVariable("PATH");
        return (from p in values?.Split(Path.PathSeparator)
            select Path.Combine(p,
                fileName ?? string.Empty)).FirstOrDefault(File.Exists);
    }
}