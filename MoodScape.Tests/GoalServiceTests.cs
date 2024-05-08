using System;
using MoodScape.Data.Enums;
using MoodScape.Data.Models;
using MoodScape.Data.Repositories;
using MoodScape.Logic.Services;
using Moq;

namespace MoodScape.Tests;

public class GoalServiceTests
{
    private Mock<IGenericRepository<Goal>> mockGoalRepository;
    private GoalService goalService;

    public GoalServiceTests()
    {
        mockGoalRepository = new Mock<IGenericRepository<Goal>>();
        goalService = new GoalService(mockGoalRepository.Object);
    }

    [Fact]
    public async Task AddGoalAsync_ShouldAddGoal_WhenGoalIsValid()
    {
        var goal = new Goal
        {
            Id = 1,
            Name = "Learn C#",
            Description = "Complete a course on C#",
            Deadline = DateTime.Now.AddDays(30),
            Status = GoalStatus.NotStarted,
            UserId = 1
        };

        await goalService.AddGoalAsync(goal);

        mockGoalRepository.Verify(repo => repo.AddAsync(It.IsAny<Goal>()), Times.Once);
    }

    [Fact]
    public async Task UpdateGoalAsync_ShouldUpdateGoal_WhenGoalIsValid()
    {
        var goal = new Goal
        {
            Id = 1,
            Name = "Learn C#",
            Description = "Complete the advanced section",
            Deadline = DateTime.Now.AddDays(15),
            Status = GoalStatus.InProgress,
            UserId = 1
        };

        await goalService.UpdateGoalAsync(goal);

        mockGoalRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Goal>()), Times.Once);
    }

    [Fact]
    public async Task DeleteGoalAsync_ShouldDeleteGoal_WhenGoalExists()
    {
        var goal = new Goal { Id = 1 };
        mockGoalRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(goal);

        await goalService.DeleteGoalAsync(1);

        mockGoalRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Goal>()), Times.Once);
    }

    [Fact]
    public async Task GetGoalByIdAsync_ShouldReturnGoal_WhenGoalExists()
    {
        var expectedGoal = new Goal { Id = 1, UserId = 1 };
        mockGoalRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(expectedGoal);

        var result = await goalService.GetGoalByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetAllGoalsByUserIdAsync_ShouldReturnGoals_WhenGoalsExist()
    {
        var goals = new List<Goal>
        {
            new Goal { Id = 1, UserId = 1 },
            new Goal { Id = 2, UserId = 1 }
        };
        mockGoalRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(goals);

        var result = await goalService.GetAllGoalsByUserIdAsync(1);

        Assert.Equal(2, result.Count());
    }


    [Fact]
    public async Task GetAchievedGoalsAsync_ShouldReturnCompletedGoals_WhenTheyExist()
    {
        var goals = new List<Goal>
        {
            new Goal { Id = 1, UserId = 1, Status = GoalStatus.Completed },
            new Goal { Id = 2, UserId = 1, Status = GoalStatus.InProgress }
        };
        mockGoalRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(goals);

        var result = await goalService.GetAchievedGoalsAsync(1);

        Assert.Single(result);
        Assert.Equal(GoalStatus.Completed, result.First().Status);
    }

    [Fact]
    public async Task GetPendingGoalsAsync_ShouldReturnInProgressGoals_WhenTheyExist()
    {
        var goals = new List<Goal>
        {
            new Goal { Id = 1, UserId = 1, Status = GoalStatus.Completed },
            new Goal { Id = 2, UserId = 1, Status = GoalStatus.InProgress }
        };
        mockGoalRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(goals);

        var result = await goalService.GetPendingGoalsAsync(1);

        Assert.Single(result);
        Assert.Equal(GoalStatus.InProgress, result.First().Status);
    }
}

