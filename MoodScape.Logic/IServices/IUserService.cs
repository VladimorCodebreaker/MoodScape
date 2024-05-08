using System;
using MoodScape.Data.Models;

namespace MoodScape.Logic.IServices;

public interface IUserService
{
    Task RegisterUserAsync(User user);
    Task<User> LoginUserAsync(string email, string password);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int userId);
    Task<User> GetUserByIdAsync(int userId);
    Task<User> GetUserByEmailAsync(string email);
    // Password Reset
    // Email Verification
}

