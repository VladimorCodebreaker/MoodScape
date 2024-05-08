using System;
using MoodScape.Logic.Analysis.Models.Results;

namespace MoodScape.Logic.Analysis;

public interface ISentimentAnalysisService
{
    Task<SentimentAnalysisResult> AnalyzeTextAsync(string text);
}

