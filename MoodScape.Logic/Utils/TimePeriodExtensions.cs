using System;
using MoodScape.Data.Enums;

namespace MoodScape.Logic.Utils;

public static class TimePeriodExtensions
{
    public static string ToFriendlyString(this TimePeriod timePeriod)
    {
        switch (timePeriod)
        {
            case TimePeriod.OneWeek:
                return "1 Week";
            case TimePeriod.OneMonth:
                return "1 Month";
            case TimePeriod.ThreeMonths:
                return "3 Months";
            case TimePeriod.SixMonths:
                return "6 Months";
            case TimePeriod.OneYear:
                return "1 Year";
            default:
                return timePeriod.ToString();
        }
    }
}

