using System;

namespace API.Debug;

public readonly struct CommandResult(ExitCode exitCode, string? msg)
{
    public readonly ExitCode ExitCode = exitCode;
    public readonly string? Msg = msg;

    public CommandResult(string? msg) : this(ExitCode.Ok, msg) { }
}
