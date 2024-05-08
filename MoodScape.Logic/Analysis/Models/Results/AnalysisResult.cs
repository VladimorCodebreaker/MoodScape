using System;
using MoodScape.Logic.Analysis.Models.Contact;
using MoodScape.Logic.Analysis.Models.Visualization;

namespace MoodScape.Logic.Analysis.Models.Results;

public class AnalysisResult
{
    public IEnumerable<SentimentResult> SentimentResults { get; set; }
    public VisualizationData VisualizationData { get; set; }
    public NotificationPrompt NotificationPrompt { get; set; }
}

