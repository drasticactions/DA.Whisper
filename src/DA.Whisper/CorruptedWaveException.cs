namespace DA.Whisper;

public class CorruptedWaveException : Exception
{
    public CorruptedWaveException(string? message) : base(message)
    {
    }
}