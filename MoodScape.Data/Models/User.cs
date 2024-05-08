using System;
using System.ComponentModel.DataAnnotations;

namespace MoodScape.Data.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Username")]
    [Required(ErrorMessage = "Username is required!")]
    public string Username { get; set; }

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Email is required!")]
    [EmailAddress]
    public string Email { get; set; }

    [Display(Name = "Password")]
    [Required(ErrorMessage = "Password is required!")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Role")]
    public string Role { get; set; }

    // Relationships
    public virtual ICollection<Mood> Moods { get; set; }
    public virtual ICollection<Goal> Goals { get; set; }
    public virtual ICollection<Habit> Habits { get; set; }
}

