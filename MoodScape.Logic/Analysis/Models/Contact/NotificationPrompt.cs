using System;

namespace MoodScape.Logic.Analysis.Models.Contact;

public class NotificationPrompt
{
    public bool ShouldNotify { get; set; }
    public string RecommendationMessage { get; set; }
    public string MailToLink { get; set; }
}

