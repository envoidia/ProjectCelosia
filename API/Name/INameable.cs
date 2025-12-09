using System;
using API.Graphics;
using API.Modding;

namespace API.Name;

/// <summary>
/// An item that can be named
/// </summary>
public interface INameable {
    string KeyName { get; }

    string GetName(string color = Colors.White, GameMod? mod = null);
}
