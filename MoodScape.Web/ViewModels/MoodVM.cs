using System;
using MoodScape.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace MoodScape.Web.ViewModels;

public class MoodVM
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Mood Level is required.")]
    [Range(1, 10, ErrorMessage = "Mood Level must be between 1 and 10.")]
    [Display(Name = "Mood Level")]
    public MoodLevel MoodLevel { get; set; }

    [Display(Name = "Description")]
    public string Description { get; set; }

    [Display(Name = "Date and Time")]
    [DataType(DataType.DateTime)]
    public DateTime LogDate { get; set; } = DateTime.UtcNow;
}

