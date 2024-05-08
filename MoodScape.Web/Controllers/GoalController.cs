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
public class GoalController : Controller
{
    private readonly IGoalService _goalService;

    public GoalController(IGoalService goalService)
    {
        _goalService = goalService;
    }

    // GET: Goal/Index
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var goals = await _goalService.GetAllGoalsByUserIdAsync(userId);

        var goalViewModels = goals.Select(g => new GoalVM
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            Deadline = g.Deadline,
            Status = g.Status
        });

        return View(goalViewModels);
    }


    // GET: Goal/Add
    [HttpGet]
    public IActionResult Add()
    {
        return View(new GoalVM());
    }

    // POST: Goal/Add
    [HttpPost]
    public async Task<IActionResult> Add(GoalVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var goal = new Goal
        {
            Name = model.Name,
            Description = model.Description,
            Deadline = model.Deadline,
            Status = model.Status,
            UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
        };

        await _goalService.AddGoalAsync(goal);
        return RedirectToAction(nameof(Index));
    }

    // GET: Goal/Update/{id}
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var goal = await _goalService.GetGoalByIdAsync(id);
        if (goal == null)
        {
            return Content("Goal -> HTTP 404 :\"Not Found\"");
        }

        var model = new GoalVM
        {
            Id = goal.Id,
            Name = goal.Name,
            Description = goal.Description,
            Deadline = goal.Deadline,
            Status = goal.Status
        };

        return View(model);
    }

    // POST: Goal/Update/{id}
    [HttpPost]
    public async Task<IActionResult> Update(GoalVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var goal = await _goalService.GetGoalByIdAsync(model.Id);
        if (goal == null)
        {
            return Content("Goal -> HTTP 404 :\"Not Found\"");
        }

        goal.Name = model.Name;
        goal.Description = model.Description;
        goal.Deadline = model.Deadline;
        goal.Status = model.Status;

        await _goalService.UpdateGoalAsync(goal);
        return RedirectToAction(nameof(Index));
    }

    // GET: Goal/Delete/{id}
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var goal = await _goalService.GetGoalByIdAsync(id);
        if (goal == null)
        {
            return Content("Goal -> HTTP 404 :\"Not Found\"");
        }

        var goalVM = new GoalVM
        {
            Id = goal.Id,
            Name = goal.Name,
            Description = goal.Description,
            Deadline = goal.Deadline,
            Status = goal.Status
        };

        return View(goalVM);
    }

    // POST: Goal/Delete/{id}
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var goal = await _goalService.GetGoalByIdAsync(id);
        if (goal == null)
        {
            return Content("Goal -> HTTP 404 :\"Not Found\"");
        }

        await _goalService.DeleteGoalAsync(id);
        return RedirectToAction(nameof(Index));
    }
}

