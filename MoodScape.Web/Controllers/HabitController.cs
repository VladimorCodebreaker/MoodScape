using System;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoodScape.Data;
using MoodScape.Data.Models;
using MoodScape.Logic.IServices;
using MoodScape.Web.ViewModels;

namespace MoodScape.Web.Controllers;

[Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
public class HabitController : Controller
{
    private readonly IHabitService _habitService;

    public HabitController(IHabitService habitService)
    {
        _habitService = habitService;
    }

    // GET: Habit/Index
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var habits = await _habitService.GetAllHabitsByUserIdAsync(userId);

        var habitViewModels = habits
            .OrderByDescending(m => m.LogDate)
            .Select(h => new HabitVM
        {
            Id = h.Id,
            Name = h.Name,
            Description = h.Description,
            Frequency = h.Frequency,
            LogDate = h.LogDate
        });

        return View(habitViewModels);
    }

    // GET: Habit/Add
    [HttpGet]
    public IActionResult Add()
    {
        return View(new HabitVM());
    }

    // POST: Habit/Add
    [HttpPost]
    public async Task<IActionResult> Add(HabitVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var habit = new Habit
        {
            Name = model.Name,
            Description = model.Description,
            Frequency = model.Frequency,
            LogDate = model.LogDate,
            UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
        };

        await _habitService.AddHabitAsync(habit);
        return RedirectToAction(nameof(Index));
    }

    // GET: Habit/Update/{id}
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var habit = await _habitService.GetHabitByIdAsync(id);
        if (habit == null)
        {
            return Content("Habit -> HTTP 404 :\"Not Found\"");
        }

        var model = new HabitVM
        {
            Id = habit.Id,
            Name = habit.Name,
            Description = habit.Description,
            Frequency = habit.Frequency,
            LogDate = habit.LogDate
        };

        return View(model);
    }

    // POST: Habit/Update/{id}
    [HttpPost]
    public async Task<IActionResult> Update(HabitVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var habit = await _habitService.GetHabitByIdAsync(model.Id);
        if (habit == null)
        {
            return Content("Habit -> HTTP 404 :\"Not Found\"");
        }

        habit.Name = model.Name;
        habit.Description = model.Description;
        habit.Frequency = model.Frequency;
        habit.LogDate = model.LogDate;

        await _habitService.UpdateHabitAsync(habit);
        return RedirectToAction(nameof(Index));
    }

    // GET: Habit/Delete/{id}
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var habit = await _habitService.GetHabitByIdAsync(id);
        if (habit == null)
        {
            return Content("Habit -> HTTP 404 :\"Not Found\"");
        }

        var habitVM = new HabitVM
        {
            Id = habit.Id,
            Name = habit.Name,
            Description = habit.Description,
            Frequency = habit.Frequency,
            LogDate = habit.LogDate
        };

        return View(habitVM);
    }

    // POST: Habit/Delete/{id}
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var habit = await _habitService.GetHabitByIdAsync(id);
        if (habit == null)
        {
            return Content("Habit -> HTTP 404 :\"Not Found\"");
        }

        await _habitService.DeleteHabitAsync(id);
        return RedirectToAction(nameof(Index));
    }
}

