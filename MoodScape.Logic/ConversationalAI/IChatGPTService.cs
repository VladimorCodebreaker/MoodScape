using System;

namespace MoodScape.Logic.ConversationalAI;

public interface IChatGPTService
{
    Task<string> GetResponseAsync(string prompt);
}

