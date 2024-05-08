using System;
using MoodScape.Data.Enums;
using MoodScape.Data.Models;
using MoodScape.Logic.Analysis.Models.Results;

namespace MoodScape.Logic.IServices;

public interface IMoodService
{
    Task AddMoodAsync(Mood mood);
    Task UpdateMoodAsync(Mood mood);
    Task DeleteMoodAsync(int moodId);

    Task<Mood> GetMoodByIdAsync(int moodId);
    Task<IEnumerable<Mood>> GetAllMoodsByUserIdAsync(int userId);
    Task<IEnumerable<Mood>> GetMoodsByTimePeriodAsync(int userId, TimePeriod period);

    Task<AnalysisResult> AnalyzeMoodPatternsAsync(int userId, TimePeriod period);
    Task<string> GenerateInsightsAsync(int moodId);
}

