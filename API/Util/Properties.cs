using System;
using System.Collections.Generic;
using System.IO;

namespace API.Util;

/// <summary>
/// Utilities for working with .properties files. Based off of, but not fully adherent to, https://en.wikipedia.org/wiki/.properties
/// <para>Specification: Lines starting with # are comments and ignored.
/// Each non-empty, non-comment line starts with a key, then has an =, then a value.
/// The only supported escape sequence is \n</para>
/// </summary>
public static class Properties {
    /// <summary>
    /// When supplied to <c>AddLine</c>, adds at the end of the file
    /// </summary>
    public const int AtEnd = -1;

    /// <summary>
    /// Parses the given .properties file
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

    /// <summary>
    /// Sets the value at the given key, if it already exists
    /// todo nyi
    /// </summary>
    /// <param name="path"></param>
    /// <param name="key"></param>
    /// <param name="val"></param>
    /// <returns>True if the key existed and was set, false if it didn't exist and nothing happened</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static bool Set(string path, string key, string val) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sets the given line number to the given line
    /// </summary>
    /// <param name="path"></param>
    /// <param name="line"></param>
    /// <param name="location"></param>
    public static void SetLine(string path, string line, int location = AtEnd) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sets the value at the given key, if it already exists. Otherwise, adds it to the end of the file
    /// todo nyi
    /// </summary>
    /// <param name="path"></param>
    /// <param name="key"></param>
    /// <param name="val"></param>
    /// <exception cref="NotImplementedException"></exception>
    public static void Add(string path, string key, string val) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Inserts the given line at the given line number
    /// </summary>
    /// <param name="path"></param>
    /// <param name="line"></param>
    /// <param name="location"></param>
    public static void InsertLine(string path, string line, int location = AtEnd) {
        throw new NotImplementedException();
    }
}
