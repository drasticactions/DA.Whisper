namespace DA.Whisper;

public class LogEventArgs : EventArgs
{
    public LogLevel Level { get; }
    public string Message { get; }

    public LogEventArgs(LogLevel level, string message)
    {
        Level = level;
        Message = message.Trim();
    }

    public override string ToString()
    {
        return $"{Level}: {Message}";
    }
}