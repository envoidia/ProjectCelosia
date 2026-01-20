using System;

namespace API.Util;

/// <summary>
/// todo docs, remove if unions arrive
/// </summary>
public readonly struct Result<T>(T? error)
    where T : struct, Enum
{
    public readonly T? Error = error;
}
