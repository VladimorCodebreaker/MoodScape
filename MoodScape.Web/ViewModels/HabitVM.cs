using System;
using MoodScape.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace MoodScape.Web.ViewModels;

public class HabitVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required!")]
    [Display(Name = "Name")]
    public string Name { get; set; }

    [Display(Name = "Description")]
    public string Description { get; set; }

    [Display(Name = "Frequency")]
    public HabitFrequency Frequency { get; set; }

    [Display(Name = "Log Date")]
    [DataType(DataType.DateTime)]
    public DateTime LogDate { get; set; } = DateTime.UtcNow;
}

