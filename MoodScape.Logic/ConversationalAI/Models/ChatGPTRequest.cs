using System;

namespace MoodScape.Logic.ConversationalAI.Models;

public class ChatGPTRequest
{
    public string Prompt { get; set; }
    public double Temperature { get; set; }
    public int MaxTokens { get; set; }
}

