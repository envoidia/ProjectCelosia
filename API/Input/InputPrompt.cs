using System.Text;
using API.Extensions;

namespace API.Input;

public sealed class InputPrompt(string keyName, params Keybind[] keybinds)
{
    public MultiInputType MultiInputType = MultiInputType.Or;

    public string GetText()
    {
        StringBuilder builder = new();

        for (int i = 0; i < keybinds.Length; i++)
        {
            builder.Append(keybinds[i].GetCurrentGlyph());

            // Divider
            if (i != keybinds.Length - 1)
            {
                builder.Append(this.MultiInputType == MultiInputType.Or ? "//" : '+');
            }
        }

        return builder.Append(' ').Append(keyName.GetLang()).ToString();
    }
}