using System;
using MoodScape.Logic.IServices;
using MoodScape.Data.Repositories;
using MoodScape.Data.Models;

namespace MoodScape.Logic.Services;

public class HabitService : IHabitService
{
    private readonly IGenericRepository<Habit> _habitRepository;

    public HabitService(IGenericRepository<Habit> habitRepository)
    {
        _habitRepository = habitRepository;
    }

    // Add
    public async Task AddHabitAsync(Habit habit)
    {
        await _habitRepository.AddAsync(habit);
    }

    // Update
    public async Task UpdateHabitAsync(Habit habit)
    {
        await _habitRepository.UpdateAsync(habit);
    }

    // Delete
    public async Task DeleteHabitAsync(int habitId)
    {
        var habit = await _habitRepository.GetByIdAsync(habitId);
        if (habit != null)
        {
            await _habitRepository.DeleteAsync(habit);
        }
    }

    // Get by Id
    public async Task<Habit> GetHabitByIdAsync(int habitId)
    {
        return await _habitRepository.GetByIdAsync(habitId);
    }

    // Get All
    public async Task<IEnumerable<Habit>> GetAllHabitsByUserIdAsync(int userId)
    {
        var habits = await _habitRepository.GetAllAsync();
        return habits.Where(habit => habit.UserId == userId).ToList();
    }
}

