using System;
using System.Text;

namespace API.Input;

public class InputPrompt(string text, params Keybind[] keybinds) {
    public Keybind[] Keybinds { get; } = keybinds;

    public string GetText() {
        StringBuilder builder = new();

        foreach (Keybind keybind in this.Keybinds) {
            builder.Append(keybind.GetCurrentGlyph());
        }

        return builder.Append(' ').Append(text).ToString();
    }
}