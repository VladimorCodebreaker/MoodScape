using System;

namespace MoodScape.Logic.Analysis.Models.Visualization;

public class ChartDataset
{
    public string Label { get; set; }
    public List<int> Data { get; set; }
    public List<string> BackgroundColor { get; set; }
    public string BorderColor { get; set; }
}

