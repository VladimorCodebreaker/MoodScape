using System;
using MoodScape.Data.Enums;

namespace MoodScape.Logic.Analysis.Models.Visualization;

public class VisualizationData
{
    public TimePeriod SelectedTimePeriod { get; set; }
    public List<string> Labels { get; set; }
    public List<ChartDataset> Datasets { get; set; }
    public List<double> PositiveConfidence { get; set; }
    public List<double> NegativeConfidence { get; set; }
    public List<double> DailyAverageMoodLevels { get; set; }

    // Modal Summary (Mood Information for selected Period)
    public int TotalEntries { get; set; }
    public int PositiveEntries { get; set; }
    public int NegativeEntries { get; set; }
    public double PeriodAverageMood { get; set; }
    public string MoodTrend { get; set; }
    public string SentimentTrend { get; set; }
}

