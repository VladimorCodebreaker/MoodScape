using System;
using MoodScape.Data.Models;

namespace MoodScape.Logic.IServices;

public interface IHabitService
{
    Task AddHabitAsync(Habit habit);
    Task UpdateHabitAsync(Habit habit);
    Task DeleteHabitAsync(int habitId);

    Task<Habit> GetHabitByIdAsync(int habitId);
    Task<IEnumerable<Habit>> GetAllHabitsByUserIdAsync(int userId);
}


