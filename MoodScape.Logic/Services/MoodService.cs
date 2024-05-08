using System;
using MoodScape.Logic.IServices;
using MoodScape.Logic.Analysis;
using MoodScape.Data.Repositories;
using MoodScape.Data.Models;
using Microsoft.EntityFrameworkCore;
using MoodScape.Data.Enums;
using MoodScape.Logic.Analysis.Models.Results;
using MoodScape.Logic.Analysis.Models.Contact;
using MoodScape.Logic.ConversationalAI;

namespace MoodScape.Logic.Services;

public class MoodService : IMoodService
{
    private readonly IGenericRepository<Mood> _moodRepository;
    private readonly ISentimentAnalysisService _sentimentAnalysisService;
    private readonly IChatGPTService _chatGPTService;


    public MoodService(IGenericRepository<Mood> moodRepository, ISentimentAnalysisService sentimentAnalysisService, IChatGPTService chatGPTService)
    {
        _moodRepository = moodRepository;
        _sentimentAnalysisService = sentimentAnalysisService;
        _chatGPTService = chatGPTService;
    }

    // Add
    public async Task AddMoodAsync(Mood mood)
    {
        await _moodRepository.AddAsync(mood);
    }

    // Update
    public async Task UpdateMoodAsync(Mood mood)
    {
        await _moodRepository.UpdateAsync(mood);
    }

    // Delete
    public async Task DeleteMoodAsync(int moodId)
    {
        var mood = await _moodRepository.GetByIdAsync(moodId);
        if (mood != null)
        {
            await _moodRepository.DeleteAsync(mood);
        }
    }

    // Get by Id
    public async Task<Mood> GetMoodByIdAsync(int moodId)
    {
        return await _moodRepository.GetByIdAsync(moodId);
    }

    // Get All
    public async Task<IEnumerable<Mood>> GetAllMoodsByUserIdAsync(int userId)
    {
        var moods = await _moodRepository.GetAllAsync();
        return moods.Where(mood => mood.UserId == userId).ToList();
    }

    // Get By Period
    public async Task<IEnumerable<Mood>> GetMoodsByTimePeriodAsync(int userId, TimePeriod period)
    {
        var dateFrom = DateTime.UtcNow.AddDays(-(int)period);
        return await _moodRepository.FindByCondition(mood => mood.UserId == userId && mood.LogDate >= dateFrom)
                                    .OrderByDescending(mood => mood.LogDate)
                                    .ToListAsync();
    }

    // Analyze Mood Patterns
    public async Task<AnalysisResult> AnalyzeMoodPatternsAsync(int userId, TimePeriod period)
    {
        var recentMoods = await GetMoodsByTimePeriodAsync(userId, period);

        var sentimentResults = new List<SentimentResult>();
        double totalMoodLevel = 0;

        foreach (var mood in recentMoods)
        {
            var sentimentAnalysisResult = await _sentimentAnalysisService.AnalyzeTextAsync(mood.Description);

            sentimentResults.Add(new SentimentResult
            {
                MoodId = mood.Id,
                Sentiment = sentimentAnalysisResult.Label,
                Confidence = sentimentAnalysisResult.Score,
                LogDate = mood.LogDate,
                MoodLevel = (int)mood.MoodLevel
            });

            totalMoodLevel += (int)mood.MoodLevel;
        }

        var averageMoodLevel = totalMoodLevel / recentMoods.Count();
        var negativeSentiments = sentimentResults.Count(sr => sr.Sentiment == "NEGATIVE");
        var percentageNegative = (double)negativeSentiments / sentimentResults.Count * 100;

        var shouldNotify = percentageNegative > 70 && averageMoodLevel < 3;

        var visualizationData = VisualizationBuilder.Prepare(sentimentResults, period);

        var notificationPrompt = new NotificationPrompt
        {
            ShouldNotify = shouldNotify,
            RecommendationMessage = "Your recent mood entries show a high percentage of negative sentiment. Would you like to notify someone?",
            MailToLink = $"mailto:?subject=MoodScape Alert&body=I've been feeling down lately and thought you should know. Here's what's been going on with me: [Add details here]"
        };

        return new AnalysisResult
        {
            SentimentResults = sentimentResults,
            VisualizationData = visualizationData,
            NotificationPrompt = notificationPrompt
        };
    }

    // Return GPT Recommendations & Insights
    public async Task<string> GenerateInsightsAsync(int moodId)
    {
        var mood = await _moodRepository.GetByIdAsync(moodId);

        if (mood == null)
        {
            return "Mood entry not found.";
        }

        string chatPrompt = $"Based on the following mood entry, provide insights and suggest recommendations for improvement: \"{mood.Description}\". sThe goal is to help the user reflect on their feelings and explore ways to enhance their emotional well-being.";

        var chatGPTResponse = await _chatGPTService.GetResponseAsync(chatPrompt);

        return chatGPTResponse;
    }
}

