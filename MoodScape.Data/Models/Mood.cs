using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MoodScape.Data.Enums;

namespace MoodScape.Data.Models;

public class Mood
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Mood Level")]
    [Required(ErrorMessage = "Mood Level is required!")]
    [Range(1, 10, ErrorMessage = "Must be between 1 to 10")]
    public MoodLevel MoodLevel { get; set; }

    [Display(Name = "Description")]
    public string Description { get; set; }

    public DateTime LogDate { get; set; }

    // Relationships
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}

