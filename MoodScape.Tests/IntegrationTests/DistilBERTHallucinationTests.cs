using System;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using MoodScape.Logic.Analysis;
using Xunit.Abstractions;

namespace MoodScape.Tests.IntegrationTests;

public class DistilBERTHallucinationTests
{
    private readonly ISentimentAnalysisService _sentimentAnalysisService;
    private readonly ITestOutputHelper _output;
    private readonly IConfiguration _configuration;

    public DistilBERTHallucinationTests(ITestOutputHelper output)
    {
        _output = output;

        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(projectRoot)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        _configuration = builder.Build();

        var apiKey = _configuration["HuggingFaceAPI:ApiKey"];
        _sentimentAnalysisService = new SentimentAnalysisService(apiKey);
    }

    [Fact]
    public async Task CalculateErrorPercentageOfMoodEntries()
    {
        var testCases = ReadTestCasesFromCsv("IntegrationTests/TestData/mood_entries.csv");

        int incorrectPredictions = 0;

        foreach (var testCase in testCases)
        {
            try
            {
                var result = await _sentimentAnalysisService.AnalyzeTextAsync(testCase.MoodText);
                if (result.Label != testCase.ExpectedLabel)
                {
                    incorrectPredictions++;
                }
                _output.WriteLine($"Mood Text: '{testCase.MoodText}' Expected: {testCase.ExpectedLabel}, Predicted: {result.Label}");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Error processing text '{testCase.MoodText}': {ex.Message}");
            }
        }

        int totalPredictions = testCases.Count;
        double errorPercentage = (double)incorrectPredictions / totalPredictions * 100;

        _output.WriteLine($"Error Percentage: {errorPercentage}%");
    }

    private List<(string MoodText, string ExpectedLabel)> ReadTestCasesFromCsv(string filePath)
    {
        var testCases = new List<(string MoodText, string ExpectedLabel)>();
        var path = Path.Combine(Directory.GetCurrentDirectory(), filePath);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };

        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, config))
        {
            var records = csv.GetRecords<MoodEntry>();
            foreach (var record in records)
            {
                testCases.Add((record.MoodText, record.ExpectedLabel));
            }
        }

        return testCases;
    }

    private class MoodEntry
    {
        public string MoodText { get; set; }
        public string ExpectedLabel { get; set; }
    }
}

