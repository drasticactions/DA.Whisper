namespace DA.Whisper;

public record SegmentData(string Text, TimeSpan Start, TimeSpan End, float MinProbability, float MaxProbability, float Probability, string Language);