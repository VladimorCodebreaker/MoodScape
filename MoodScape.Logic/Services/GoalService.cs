using System;
using MoodScape.Data.Enums;
using MoodScape.Logic.IServices;
using MoodScape.Data.Repositories;
using MoodScape.Data.Models;

namespace MoodScape.Logic.Services;

public class GoalService : IGoalService
{
    private readonly IGenericRepository<Goal> _goalRepository;

    public GoalService(IGenericRepository<Goal> goalRepository)
    {
        _goalRepository = goalRepository;
    }

    // Add
    public async Task AddGoalAsync(Goal goal)
    {
        await _goalRepository.AddAsync(goal);
    }

    // Update
    public async Task UpdateGoalAsync(Goal goal)
    {
        await _goalRepository.UpdateAsync(goal);
    }

    // Delete
    public async Task DeleteGoalAsync(int goalId)
    {
        var goal = await _goalRepository.GetByIdAsync(goalId);
        if (goal != null)
        {
            await _goalRepository.DeleteAsync(goal);
        }
    }

    // Get by Id
    public async Task<Goal> GetGoalByIdAsync(int goalId)
    {
        return await _goalRepository.GetByIdAsync(goalId);
    }

    // Get All
    public async Task<IEnumerable<Goal>> GetAllGoalsByUserIdAsync(int userId)
    {
        var goals = await _goalRepository.GetAllAsync();
        return goals.Where(goal => goal.UserId == userId).ToList();
    }

    // Get All [Completed]
    public async Task<IEnumerable<Goal>> GetAchievedGoalsAsync(int userId)
    {
        var goals = await _goalRepository.GetAllAsync();
        return goals.Where(goal => goal.UserId == userId && goal.Status == GoalStatus.Completed).ToList();
    }

    // Get All [InProgress]
    public async Task<IEnumerable<Goal>> GetPendingGoalsAsync(int userId)
    {
        var goals = await _goalRepository.GetAllAsync();
        return goals.Where(goal => goal.UserId == userId && goal.Status == GoalStatus.InProgress).ToList();
    }
}

