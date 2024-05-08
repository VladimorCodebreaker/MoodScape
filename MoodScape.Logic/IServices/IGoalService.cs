using System;
using MoodScape.Data.Models;

namespace MoodScape.Logic.IServices;

public interface IGoalService
{
    Task AddGoalAsync(Goal goal);
    Task UpdateGoalAsync(Goal goal);
    Task DeleteGoalAsync(int goalId);

    Task<Goal> GetGoalByIdAsync(int goalId);
    Task<IEnumerable<Goal>> GetAllGoalsByUserIdAsync(int userId);
    Task<IEnumerable<Goal>> GetAchievedGoalsAsync(int userId);
    Task<IEnumerable<Goal>> GetPendingGoalsAsync(int userId);
}

