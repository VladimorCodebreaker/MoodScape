using System;
using MoodScape.Logic.Analysis.Models.Results;

namespace MoodScape.Web.ViewModels;

public class MoodAnalysisVM
{
    public IEnumerable<MoodVM> Moods { get; set; }
    public AnalysisResult AnalysisResult { get; set; }
}

