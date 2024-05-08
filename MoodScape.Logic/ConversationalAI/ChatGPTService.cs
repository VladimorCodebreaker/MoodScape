using System;
using Microsoft.Extensions.Configuration;
using MoodScape.Logic.ConversationalAI.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Polly;
using System.Text.Json;
using System.Security.Policy;

namespace MoodScape.Logic.ConversationalAI;

public class ChatGPTService : IChatGPTService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public ChatGPTService(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _apiKey = configuration["OpenAI:ApiKey"];
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<string> GetResponseAsync(string prompt)
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        var requestBody = new
        {
            model = "gpt-4",
            messages = new[]
                    {
                new { role = "user", content = prompt }
            },
            temperature = 0.7
        };

        HttpResponseMessage response = await retryPolicy.ExecuteAsync(() => _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody));

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent);

        if (jsonResponse != null && jsonResponse.TryGetValue("choices", out JsonElement choices))
        {
            var firstChoice = choices.EnumerateArray().FirstOrDefault();
            if (firstChoice.TryGetProperty("message", out JsonElement messageElement))
            {
                if (messageElement.TryGetProperty("content", out JsonElement contentElement))
                {
                    var messageContent = contentElement.GetString();
                    return messageContent;
                }
            }
        }

        return "No response found.";
    }
}

