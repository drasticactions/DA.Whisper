namespace DA.Whisper;

public class NotSupportedWaveException : Exception
{
    public NotSupportedWaveException(string? message) : base(message)
    {
    }
}