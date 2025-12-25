using System;
using System.Collections.Generic;
using System.IO;

namespace API.Util;

public static class Properties {
    /// <summary>
    /// Parses the given .properties file. Based off of, but not fully adherent to, https://en.wikipedia.org/wiki/.properties
    /// <para>Specification: Lines starting with # are comments and ignored.
    /// Each non-empty, non-comment line starts with a key, then has an =, then a value.
    /// The only supported escape sequence is \n</para>
    /// </summary>
    /// <returns>A dictionary formed from the given file</returns>
    public static Dictionary<string, string> Parse(string path) {
        Dictionary<string, string> dict = [];
        string[] lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++) {
            // Ignore comments and blank lines
            if (lines[i].StartsWith('#') || lines[i].IsWhiteSpace()) continue;

            // Split key and value
            string[] parts = lines[i].Split('=', 2);

            if (parts.Length == 2) dict.Add(parts[0], parts[1].Replace("\\n", "\n"));
            else throw new FormatException(
                $"Exception parsing properties file {path}: line {i}, \"{lines[i]}\" is not a comment and is missing separator =");
        }

        return dict;
    }
}
