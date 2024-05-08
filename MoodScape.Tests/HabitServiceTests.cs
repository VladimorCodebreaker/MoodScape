using System;
using MoodScape.Data.Enums;
using MoodScape.Data.Models;
using MoodScape.Data.Repositories;
using MoodScape.Logic.Services;
using Moq;

namespace MoodScape.Tests;

public class HabitServiceTests
{
    private Mock<IGenericRepository<Habit>> mockHabitRepository;
    private HabitService habitService;

    public HabitServiceTests()
    {
        mockHabitRepository = new Mock<IGenericRepository<Habit>>();
        habitService = new HabitService(mockHabitRepository.Object);
    }

    [Fact]
    public async Task AddHabitAsync_ShouldAddHabit_WhenHabitIsValid()
    {
        var habit = new Habit
        {
            Id = 1,
            Name = "Morning Run",
            Description = "Run for 30 minutes every morning",
            Frequency = HabitFrequency.Daily,
            LogDate = DateTime.Now,
            UserId = 1
        };

        await habitService.AddHabitAsync(habit);

        mockHabitRepository.Verify(repo => repo.AddAsync(It.IsAny<Habit>()), Times.Once);
    }

    [Fact]
    public async Task UpdateHabitAsync_ShouldUpdateHabit_WhenHabitIsValid()
    {
        var habit = new Habit
        {
            Id = 1,
            Name = "Evening Meditation",
            Description = "Meditate for 15 minutes every evening",
            Frequency = HabitFrequency.Daily,
            LogDate = DateTime.Now,
            UserId = 1
        };

        await habitService.UpdateHabitAsync(habit);

        mockHabitRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Habit>()), Times.Once);
    }

    [Fact]
    public async Task DeleteHabitAsync_ShouldDeleteHabit_WhenHabitExists()
    {
        var habit = new Habit { Id = 1 };
        mockHabitRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(habit);

        await habitService.DeleteHabitAsync(1);

        mockHabitRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Habit>()), Times.Once);
    }

    [Fact]
    public async Task GetHabitByIdAsync_ShouldReturnHabit_WhenHabitExists()
    {
        var expectedHabit = new Habit { Id = 1, UserId = 1 };
        mockHabitRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(expectedHabit);

        var result = await habitService.GetHabitByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetAllHabitsByUserIdAsync_ShouldReturnHabits_WhenHabitsExist()
    {
        var habits = new List<Habit>
        {
            new Habit { Id = 1, UserId = 1 },
            new Habit { Id = 2, UserId = 1 }
        };
        mockHabitRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(habits);

        var result = await habitService.GetAllHabitsByUserIdAsync(1);

        Assert.Equal(2, result.Count());
    }
}

