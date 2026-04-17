namespace API.Debug;

public readonly struct LogMessage(string msg, string source, LogLevel logLevel = LogLevel.Info)
{
    public readonly string Msg = msg;
    public readonly string Source = source;
    public readonly LogLevel LogLevel = logLevel;
}
