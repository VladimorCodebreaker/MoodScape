using System;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoodScape.Data;
using MoodScape.Data.Enums;
using MoodScape.Data.Models;
using MoodScape.Logic.IServices;
using MoodScape.Web.ViewModels;

namespace MoodScape.Web.Controllers;

[Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
public class MoodController : Controller
{
    private readonly IMoodService _moodService;

    public MoodController(IMoodService moodService)
    {
        _moodService = moodService;
    }

    // GET: Mood/Index
    [HttpGet]
    public async Task<IActionResult> Index(TimePeriod period = TimePeriod.OneWeek)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var moods = await _moodService.GetAllMoodsByUserIdAsync(userId);

        var analysisResult = await _moodService.AnalyzeMoodPatternsAsync(userId, period);
        analysisResult.VisualizationData.SelectedTimePeriod = period;

        var viewModel = new MoodAnalysisVM
        {
            Moods = moods
                    .OrderByDescending(m => m.LogDate)
                    .Select(m => new MoodVM
                    {
                        Id = m.Id,
                        MoodLevel = m.MoodLevel,
                        Description = m.Description,
                        LogDate = m.LogDate
                    }),
            AnalysisResult = analysisResult
        };

        return View(viewModel);
    }

    // POST: Mood/UpdatePeriod
    [HttpPost]
    public async Task<IActionResult> UpdatePeriod(TimePeriod selectedPeriod)
    {
        return RedirectToAction("Index", new { period = selectedPeriod });
    }

    // GET: Mood/Add
    [HttpGet]
    public IActionResult Add()
    {
        return View(new MoodVM());
    }

    // POST: Mood/Add
    [HttpPost]
    public async Task<IActionResult> Add(MoodVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var mood = new Mood
        {
            MoodLevel = model.MoodLevel,
            Description = model.Description,
            UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
            LogDate = model.LogDate
        };

        await _moodService.AddMoodAsync(mood);
        return RedirectToAction(nameof(Index));
    }

    // GET: Mood/Update/{id}
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var mood = await _moodService.GetMoodByIdAsync(id);
        if (mood == null)
        {
            return Content("Mood -> HTTP 404 :\"Not Found\"");
        }

        var model = new MoodVM
        {
            Id = mood.Id,
            MoodLevel = mood.MoodLevel,
            Description = mood.Description,
            LogDate = mood.LogDate
        };

        return View(model);
    }

    // POST: Mood/Update/{id}
    [HttpPost]
    public async Task<IActionResult> Update(MoodVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var mood = await _moodService.GetMoodByIdAsync(model.Id);
        if (mood == null)
        {
            return Content("Mood -> HTTP 404 :\"Not Found\"");
        }

        mood.MoodLevel = model.MoodLevel;
        mood.Description = model.Description;
        mood.LogDate = model.LogDate;

        await _moodService.UpdateMoodAsync(mood);
        return RedirectToAction(nameof(Index));
    }

    // GET: Mood/Delete/{id}
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var mood = await _moodService.GetMoodByIdAsync(id);
        if (mood == null)
        {
            return Content("Mood -> HTTP 404 :\"Not Found\"");
        }

        var moodVM = new MoodVM
        {
            Id = mood.Id,
            MoodLevel = mood.MoodLevel,
            Description = mood.Description,
            LogDate = mood.LogDate
        };

        return View(moodVM);
    }

    // POST: Mood/Delete/{id}
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var mood = await _moodService.GetMoodByIdAsync(id);
        if (mood == null)
        {
            return Content("Mood -> HTTP 404 :\"Not Found\"");
        }

        await _moodService.DeleteMoodAsync(id);
        return RedirectToAction(nameof(Index));
    }

    // GET: Mood/GenerateInsights/{id}
    [HttpGet]
    public async Task<IActionResult> GenerateInsights(int id)
    {
        var conversationResponse = await _moodService.GenerateInsightsAsync(id);

        if (conversationResponse == "Mood entry not found.")
        {
            return Content("Mood -> HTTP 404 :\"Not Found\"");
        }

        var model = new ConversationVM
        {
            MoodId = id,
            ChatResponse = conversationResponse
        };

        return View("ChatGPT", model);
    }
}

