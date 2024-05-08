using System.Linq.Expressions;
using MoodScape.Data.Enums;
using MoodScape.Data.Models;
using MoodScape.Data.Repositories;
using MoodScape.Logic.Analysis;
using MoodScape.Logic.ConversationalAI;
using MoodScape.Logic.Services;
using Moq;

namespace MoodScape.Tests;

public class MoodServiceTests
{
    private readonly Mock<IGenericRepository<Mood>> mockMoodRepository;
    private readonly Mock<ISentimentAnalysisService> mockSentimentAnalysisService;
    private readonly Mock<IChatGPTService> mockChatGPTService;
    private readonly MoodService moodService;

    public MoodServiceTests()
    {
        mockMoodRepository = new Mock<IGenericRepository<Mood>>();
        mockSentimentAnalysisService = new Mock<ISentimentAnalysisService>();
        mockChatGPTService = new Mock<IChatGPTService>();
        moodService = new MoodService(mockMoodRepository.Object, mockSentimentAnalysisService.Object, mockChatGPTService.Object);
    }

    [Fact]
    public async Task AddMoodAsync_ShouldAddMood_WhenMoodIsValid()
    {
        var mood = new Mood
        {
            Id = 1,
            MoodLevel = MoodLevel.High,
            Description = "Feeling great!",
            LogDate = DateTime.Now,
            UserId = 1
        };

        await moodService.AddMoodAsync(mood);

        mockMoodRepository.Verify(m => m.AddAsync(It.IsAny<Mood>()), Times.Once);
    }

    [Fact]
    public async Task UpdateMoodAsync_ShouldUpdateMood_WhenMoodIsValid()
    {
        var mood = new Mood
        {
            Id = 1,
            MoodLevel = MoodLevel.Neutral,
            Description = "Feeling okay.",
            LogDate = DateTime.Now,
            UserId = 1
        };

        await moodService.UpdateMoodAsync(mood);

        mockMoodRepository.Verify(m => m.UpdateAsync(It.IsAny<Mood>()), Times.Once);
    }

    [Fact]
    public async Task DeleteMoodAsync_ShouldDeleteMood_WhenMoodExists()
    {
        mockMoodRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Mood());

        await moodService.DeleteMoodAsync(1);

        mockMoodRepository.Verify(m => m.DeleteAsync(It.IsAny<Mood>()), Times.Once);
    }

    [Fact]
    public async Task GetMoodByIdAsync_ShouldReturnMood_WhenMoodExists()
    {
        var expectedMood = new Mood
        {
            Id = 1,
            MoodLevel = MoodLevel.High,
            Description = "Feeling great!",
            LogDate = DateTime.Now,
            UserId = 1
        };
        mockMoodRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(expectedMood);

        var result = await moodService.GetMoodByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetAllMoodsByUserIdAsync_ShouldReturnAllMoodsForUser()
    {
        var moods = new List<Mood>
        {
            new Mood { UserId = 1, MoodLevel = MoodLevel.High, Description = "Good", LogDate = DateTime.Now },
            new Mood { UserId = 1, MoodLevel = MoodLevel.Low, Description = "Bad", LogDate = DateTime.Now.AddDays(-1) }
        };
        mockMoodRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(moods);

        var result = await moodService.GetAllMoodsByUserIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
}

