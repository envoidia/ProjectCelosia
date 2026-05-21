using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace API.Util;

/// <summary>
/// Utilities for working with .properties files.
/// Based off of, but not fully adherent to, https://en.wikipedia.org/wiki/.properties
/// <para>Specification: Lines starting with # are comments and ignored.
/// Each non-empty, non-comment line starts with a key, then has an =, then a value.
/// The only supported escape sequence is \n</para>
/// </summary>
public static class Properties
{
    /// <summary>
    /// Tries to parse the given .properties file
    /// </summary>
    /// <returns>
    /// null if the file was parsed correctly, otherwise an exception
    /// </returns>
    public static Exception? TryParse(string path, out Dictionary<string, string>? dict)
    {
        dict = [];
        ReadOnlySpan<string> lines;

        try
        {
            lines = File.ReadAllLines(path);
        }
        catch (Exception e)
        {
            return e;
        }

        for (int i = 0; i < lines.Length; i++)
        {
            // Ignore comments and blank lines
            if (lines[i].StartsWith('#') || lines[i].IsWhiteSpace())
            {
                continue;
            }

            // Split key and value
            ReadOnlySpan<string> parts = lines[i].Split('=', 2);

            if (parts.Length == 2)
            {
                dict.Add(parts[0], parts[1]
                    // Unescape newlines
                    .Replace("\\n", "\n"));
                continue;
            }

            dict = null;
            return new FormatException($"Malformed properties file {path}: line {i
                + 1}, \"{lines[i]}\" is not a comment and is missing separator =");
        }

        return null;
    }

    /// <summary>
    /// Creates a .properties file from the given dictionary
    /// </summary>
    public static void Create(string path, Dictionary<string, string> props)
    {
        const int Cap = 300;
        StringBuilder sb = new(Cap);

        foreach (KeyValuePair<string, string> kvp in props)
        {
            sb.Append($"{kvp.Key}={kvp.Value}\n");
        }

        File.WriteAllText(path, sb.ToString());
    }
}
