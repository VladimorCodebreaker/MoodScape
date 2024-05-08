using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoodScape.Logic.IServices;
using MoodScape.Data.Repositories;
using MoodScape.Data.Models;
using MoodScape.Data;

namespace MoodScape.Logic.Services;

public class UserService : IUserService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly PasswordHasher<User> _passwordHasher;

    public UserService(IGenericRepository<User> userRepository, PasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    // Register
    public async Task RegisterUserAsync(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
        {
            throw new ArgumentException("Email and password are required.");
        }

        var existingUser = await _userRepository.FindByCondition(u => u.Email == user.Email).FirstOrDefaultAsync();
        if (existingUser != null)
        {
            throw new InvalidOperationException("A user with the given email already exists.");
        }

        user.Password = _passwordHasher.HashPassword(user, user.Password);
        user.Role = UserRoles.User;
        await _userRepository.AddAsync(user);
    }

    // Login
    public async Task<User> LoginUserAsync(string email, string password)
    {
        var user = await _userRepository.FindByCondition(u => u.Email == email).FirstOrDefaultAsync();
        if (user != null && _passwordHasher.VerifyHashedPassword(user, user.Password, password) == PasswordVerificationResult.Success)
        {
            return user;
        }
        return null;
    }

    // Update
    public async Task UpdateUserAsync(User user)
    {
        var existingUser = await _userRepository.GetByIdAsync(user.Id);
        if (existingUser != null)
        {
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;

            await _userRepository.UpdateAsync(existingUser);
        }
    }

    // Delete
    public async Task DeleteUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user != null)
        {
            await _userRepository.DeleteAsync(user);
        }
    }

    // Get by Id
    public async Task<User> GetUserByIdAsync(int userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }

    // Get by Email
    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _userRepository.FindByCondition(u => u.Email == email).FirstOrDefaultAsync();
    }
}

