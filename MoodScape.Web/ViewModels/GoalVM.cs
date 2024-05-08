using System;
using System.ComponentModel.DataAnnotations;
using MoodScape.Data.Enums;

namespace MoodScape.Web.ViewModels;

public class GoalVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required!")]
    [Display(Name = "Name")]
    public string Name { get; set; }

    [Display(Name = "Description")]
    public string Description { get; set; }

    [Display(Name = "Deadline")]
    [DataType(DataType.DateTime)]
    public DateTime Deadline { get; set; }

    [Display(Name = "Status")]
    public GoalStatus Status { get; set; } = GoalStatus.NotStarted;
}

