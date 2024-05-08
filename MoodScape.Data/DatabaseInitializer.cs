using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoodScape.Data.Enums;
using MoodScape.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MoodScape.Data;

public class DatabaseInitializer
{
    public static void SeedData(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<DatabaseContext>();
            var passwordHasher = new PasswordHasher<User>();
            var logger = scope.ServiceProvider.GetService<ILogger<DatabaseInitializer>>();

            context.Database.EnsureCreated();

            logger.LogInformation("Database created");

            SeedUsers(context, passwordHasher);
            SeedMoods(context);
            SeedGoals(context);
            SeedHabits(context);
        }
    }

    private static void SeedUsers(DatabaseContext context, IPasswordHasher<User> passwordHasher)
    {
        if (!context.Users.Any())
        {
            var users = new List<User>
            {
                new User { Username = "User", Email = "user@gmail.com", Password = passwordHasher.HashPassword(null, "1234"), Role = "User" },
                new User { Username = "Admin", Email = "admin@gmail.com", Password = passwordHasher.HashPassword(null, "1234"), Role = "Admin" },
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }

    private static void SeedMoods(DatabaseContext context)
    {
        if (!context.Moods.Any())
        {
            var user = context.Users.FirstOrDefault(u => u.Username == "User");
            {
                var moods = new List<Mood>
                {
                    new Mood { MoodLevel = MoodLevel.Neutral, Description = "Feeling okay", LogDate = DateTime.Now, UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.SlightlyLow, Description = "Feeling really heated and outraged today.", LogDate = DateTime.Now.AddDays(-1), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.VeryLow, Description = "I am so annoyed and fuming right now.", LogDate = DateTime.Now.AddDays(-2), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.Low, Description = "Rage is all I feel, boiling with anger.", LogDate = DateTime.Now.AddDays(-3), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.SlightlyLow, Description = "Feeling lonely and isolated lately.", LogDate = DateTime.Now.AddDays(-4), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.SlightlyLow, Description = "I've been feeling very secluded and alone.", LogDate = DateTime.Now.AddDays(-5), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.VeryLow, Description = "It's like I'm abandoned, no one cares.", LogDate = DateTime.Now.AddDays(-6), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.Low, Description = "Feeling unhappy and sorrowful today.", LogDate = DateTime.Now.AddDays(-7), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.VeryLow, Description = "My heart feels so heavy and heartbroken.", LogDate = DateTime.Now.AddDays(-8), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.VeryLow, Description = "I'm grieving and mourning, feeling very downcast.", LogDate = DateTime.Now.AddDays(-9), UserId = user.Id},

                    new Mood { MoodLevel = MoodLevel.SlightlyLow, Description = "Today I feel completely overwhelmed with tasks.", LogDate = DateTime.Now.AddDays(-10), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.SlightlyLow, Description = "Feeling very tense and strained after the meeting.", LogDate = DateTime.Now.AddDays(-11), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.VeryLow, Description = "I'm feeling so depressed and empty inside.", LogDate = DateTime.Now.AddDays(-12), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.Low, Description = "Today, I just feel worthless and disinterested in everything.", LogDate = DateTime.Now.AddDays(-13), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.Low, Description = "Feeling anxious and restless about the upcoming event.", LogDate = DateTime.Now.AddDays(-14), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.MildlyLow, Description = "I am so wound up, I can't relax at all.", LogDate = DateTime.Now.AddDays(-15), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.MildlyLow, Description = "Today, I am just furious and resentful about what happened.", LogDate = DateTime.Now.AddDays(-16), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.SlightlyLow, Description = "Feeling really annoyed and seeing red over the situation.", LogDate = DateTime.Now.AddDays(-17), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.Low, Description = "I feel so lonely and secluded, missing companionship.", LogDate = DateTime.Now.AddDays(-18), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.Neutral, Description = "There's a feeling of melancholy, just gloomy and tearful.", LogDate = DateTime.Now.AddDays(-19), UserId = user.Id},

                    new Mood { MoodLevel = MoodLevel.Neutral, Description = "Feeling strained and a bit exhausted with work.", LogDate = DateTime.Now.AddDays(-20), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.Neutral, Description = "Today feels quite overwhelming, but managing to keep calm.", LogDate = DateTime.Now.AddDays(-21), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.SlightlyHigh, Description = "Feeling downcast and sorrowful, a bit unhappy with life currently.", LogDate = DateTime.Now.AddDays(-22), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.MildlyLow, Description = "I'm engulfed in sadness, feeling very heartbroken and grieving.", LogDate = DateTime.Now.AddDays(-23), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.SlightlyLow, Description = "Today, I am just irritated and fuming over recent events.", LogDate = DateTime.Now.AddDays(-24), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.ModeratelyHigh, Description = "Feeling a bit anxious but still hopeful about the future.", LogDate = DateTime.Now.AddDays(-25), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.Neutral, Description = "Today is challenging; feeling overwhelmed and swamped at work.", LogDate = DateTime.Now.AddDays(-26), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.High, Description = "Feeling somewhat isolated but trying to stay positive.", LogDate = DateTime.Now.AddDays(-27), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.SlightlyHigh, Description = "Feeling totally burnt out and exhausted, need a break.", LogDate = DateTime.Now.AddDays(-28), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.High, Description = "Despite some stress, feeling happy and accomplished.", LogDate = DateTime.Now.AddDays(-29), UserId = user.Id},
                    new Mood { MoodLevel = MoodLevel.Neutral, Description = "Feeling a bit lost and alone, needing some company.", LogDate = DateTime.Now.AddDays(-30), UserId = user.Id},

                };
                context.Moods.AddRange(moods);
                context.SaveChanges();
            }
        }
    }

    private static void SeedGoals(DatabaseContext context)
    {
        if (!context.Goals.Any())
        {
            var user = context.Users.FirstOrDefault(u => u.Username == "User");
            {
                var goals = new List<Goal>
                {
                    new Goal { Name = "Exercise", Description = "Regular exercise", Deadline = DateTime.Now.AddDays(30), UserId = user.Id },
                };
                context.Goals.AddRange(goals);
                context.SaveChanges();
            }
        }
    }

    private static void SeedHabits(DatabaseContext context)
    {
        if (!context.Habits.Any())
        {
            var user = context.Users.FirstOrDefault(u => u.Username == "User");
            {
                var habits = new List<Habit>
                {
                    new Habit { Name = "Reading", Description = "Read daily", Frequency = HabitFrequency.Daily, UserId = user.Id },
                };
                context.Habits.AddRange(habits);
                context.SaveChanges();
            }
        }
    }
}

