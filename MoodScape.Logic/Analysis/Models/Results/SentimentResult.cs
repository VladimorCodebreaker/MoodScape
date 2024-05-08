using System;

namespace MoodScape.Logic.Analysis.Models.Results;

public class SentimentResult
{
    public int MoodId { get; set; }
    public string Sentiment { get; set; }
    public double Confidence { get; set; }
    public DateTime LogDate { get; set; }
    public int MoodLevel { get; set; }
}

