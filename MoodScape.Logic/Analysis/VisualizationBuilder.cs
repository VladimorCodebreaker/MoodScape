using System;
using MoodScape.Data.Enums;
using MoodScape.Logic.Analysis.Models.Results;
using MoodScape.Logic.Analysis.Models.Visualization;

namespace MoodScape.Logic.Analysis;

public class VisualizationBuilder
{
    public static VisualizationData Prepare(IEnumerable<SentimentResult> sentimentResults, TimePeriod period)
    {
        DateTime startDate = DateTime.UtcNow.AddDays(-(int)period);

        var filteredSentimentResults = sentimentResults
            .Where(sr => sr.LogDate >= startDate)
            .OrderBy(sr => sr.LogDate)
            .ToList();

        if (!filteredSentimentResults.Any())
        {
            return new VisualizationData
            {
                SelectedTimePeriod = period,
                Labels = new List<string>(),
                Datasets = new List<ChartDataset>(),
                PositiveConfidence = new List<double>(),
                NegativeConfidence = new List<double>(),
                DailyAverageMoodLevels = new List<double>(),
                TotalEntries = 0,
                PositiveEntries = 0,
                NegativeEntries = 0,
                PeriodAverageMood = 0,
                MoodTrend = "Neutral",
                SentimentTrend = "Unchanged"
            };
        }

        var labels = filteredSentimentResults
            .Select(sr => sr.LogDate.ToString("dd/MM/yyyy"))
            .Distinct()
            .ToList();

        var positiveData = new List<int>();
        var negativeData = new List<int>();
        var positiveConfidence = new List<double>();
        var negativeConfidence = new List<double>();
        var averageMoodLevels = new List<double>();

        foreach (var date in labels)
        {
            var dateParsed = DateTime.ParseExact(date, "dd/MM/yyyy", null);
            var positiveEntries = filteredSentimentResults
                .Where(sr => sr.LogDate.Date == dateParsed && sr.Sentiment == "POSITIVE")
                .ToList();
            var negativeEntries = filteredSentimentResults
                .Where(sr => sr.LogDate.Date == dateParsed && sr.Sentiment == "NEGATIVE")
                .ToList();

            positiveData.Add(positiveEntries.Count);
            negativeData.Add(negativeEntries.Count);

            positiveConfidence.Add(positiveEntries.Any() ? positiveEntries.Average(sr => sr.Confidence) : 0);
            negativeConfidence.Add(negativeEntries.Any() ? negativeEntries.Average(sr => sr.Confidence) : 0);

            var dayEntries = filteredSentimentResults.Where(sr => sr.LogDate.Date == dateParsed).ToList();
            averageMoodLevels.Add(dayEntries.Any() ? dayEntries.Average(sr => sr.MoodLevel) : 0);
        }

        var datasets = new List<ChartDataset>
        {
            new ChartDataset
            {
                Label = "Positive",
                Data = positiveData,
                BackgroundColor = positiveConfidence.Select(c => $"rgba(75, 192, 192, {c})").ToList(),
                BorderColor = "rgba(75, 192, 192, 1)"
            },
            new ChartDataset
            {
                Label = "Negative",
                Data = negativeData,
                BackgroundColor = negativeConfidence.Select(c => $"rgba(255, 99, 132, {c})").ToList(),
                BorderColor = "rgba(255, 99, 132, 1)"
            }
        };

        var (moodTrend, sentimentTrend) = DetermineTrend(filteredSentimentResults);

        return new VisualizationData
        {
            SelectedTimePeriod = period,
            Labels = labels,
            Datasets = datasets,
            PositiveConfidence = positiveConfidence,
            NegativeConfidence = negativeConfidence,
            DailyAverageMoodLevels = averageMoodLevels,

            TotalEntries = filteredSentimentResults.Count,
            PositiveEntries = filteredSentimentResults.Count(sr => sr.Sentiment == "POSITIVE"),
            NegativeEntries = filteredSentimentResults.Count(sr => sr.Sentiment == "NEGATIVE"),
            PeriodAverageMood = filteredSentimentResults.Average(sr => sr.MoodLevel),
            MoodTrend = moodTrend,
            SentimentTrend = sentimentTrend
        };
    }

    private static (string MoodTrend, string SentimentTrend) DetermineTrend(IEnumerable<SentimentResult> filteredSentimentResults)
    {
        if (!filteredSentimentResults.Any())
        {
            return ("Neutral", "Unchanged");
        }

        if (filteredSentimentResults.Count() == 1)
        {
            return ("Single Entry", "Unchanged");
        }

        var splitIndex = filteredSentimentResults.Count() / 2;

        var firstHalfAverageMood = filteredSentimentResults.Take(splitIndex).Average(sr => sr.MoodLevel);
        var secondHalfAverageMood = filteredSentimentResults.Skip(splitIndex).Average(sr => sr.MoodLevel);
        var moodTrend = secondHalfAverageMood > firstHalfAverageMood ? "Improving" : (secondHalfAverageMood < firstHalfAverageMood ? "Deteriorating" : "Stable");

        var firstHalfPositiveCount = filteredSentimentResults.Take(splitIndex).Count(sr => sr.Sentiment == "POSITIVE");
        var secondHalfPositiveCount = filteredSentimentResults.Skip(splitIndex).Count(sr => sr.Sentiment == "POSITIVE");
        var sentimentTrend = secondHalfPositiveCount > firstHalfPositiveCount ? "Positive" : (secondHalfPositiveCount < firstHalfPositiveCount ? "Negative" : "Unchanged");

        return (moodTrend, sentimentTrend);
    }
}

