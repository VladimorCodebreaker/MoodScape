using System;
using Newtonsoft.Json;
using System.Text;
using Polly;
using MoodScape.Logic.Analysis.Models.Results;

namespace MoodScape.Logic.Analysis;

public class SentimentAnalysisService : ISentimentAnalysisService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public SentimentAnalysisService(string apiKey)
    {
        _httpClient = new HttpClient();
        _apiKey = apiKey;
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task<SentimentAnalysisResult> AnalyzeTextAsync(string text)
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        var payload = new { inputs = text };
        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        HttpResponseMessage response = await retryPolicy.ExecuteAsync(async () =>
        {
            return await _httpClient.PostAsync("https://api-inference.huggingface.co/models/distilbert-base-uncased-finetuned-sst-2-english", content);
        });

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        var sentiments = JsonConvert.DeserializeObject<List<List<SentimentAnalysisResult>>>(result);
        var highestScoreSentiment = sentiments[0].OrderByDescending(s => s.Score).First();

        return highestScoreSentiment;
    }
}

