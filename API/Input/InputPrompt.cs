using System.Text;

namespace API.Input;

public class InputPrompt(string text, params Keybind[] keybinds) {
    public string Text { get; } = text;
    public Keybind[] Keybinds { get; } = keybinds;

    public string GetText() {
        StringBuilder builder = new();

        foreach (Keybind keybind in this.Keybinds) {
            builder.Append(keybind.GetCurrentGlyph());
        }

        return builder.Append(' ').Append(this.Text).ToString();
    }
}