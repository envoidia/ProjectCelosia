using System;
using API.Modding;

namespace API.Name;

/// <summary>
/// An item that can be described, with formatting arguments and other <c>IDescribable</c>s included in the description
/// </summary>
public interface IComplexDescribable : IDescribable {
    DescData Data { get; }

    void GetFullDesc(GameMod? mod = null);
}
