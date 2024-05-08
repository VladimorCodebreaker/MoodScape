using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MoodScape.Data.Enums;

namespace MoodScape.Data.Models;

public class Habit
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Name")]
    [Required(ErrorMessage = "Name is required!")]
    public string Name { get; set; }

    [Display(Name = "Description")]
    public string Description { get; set; }

    [Display(Name = "Frequency")]
    public HabitFrequency Frequency { get; set; }

    public DateTime LogDate { get; set; }

    // Relationships
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}

