using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using MoodScape.Data;
using MoodScape.Data.Models;
using MoodScape.Data.Repositories;
using MoodScape.Logic.Services;
using Moq;

namespace MoodScape.Tests;

public class UserServiceTests
{
    private Mock<IGenericRepository<User>> mockUserRepository;
    private Mock<PasswordHasher<User>> mockPasswordHasher;
    private UserService userService;

    public UserServiceTests()
    {
        mockUserRepository = new Mock<IGenericRepository<User>>();
        var passwordHasher = new PasswordHasher<User>();
        userService = new UserService(mockUserRepository.Object, passwordHasher);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldThrowException_WhenUserExists()
    {
        var user = new User { Email = "test@example.com", Password = "Password123" };
        mockUserRepository.Setup(repo => repo.FindByCondition(It.IsAny<Expression<Func<User, bool>>>())).Returns(new List<User> { user }.AsQueryable());

        await Assert.ThrowsAsync<InvalidOperationException>(() => userService.RegisterUserAsync(user));
    }

    //[Fact]
    //public async Task RegisterUserAsync_ShouldAddUser_WhenUserDoesNotExist()
    //{
    //    var user = new User { Email = "newuser@example.com", Password = "Password123" };
    //    mockUserRepository.Setup(repo => repo.FindByCondition(It.IsAny<Expression<Func<User, bool>>>())).Returns(new List<User> { user }.AsAsyncQueryable());

    //    mockPasswordHasher.Setup(hasher => hasher.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
    //                      .Returns("hashedpassword");

    //    await userService.RegisterUserAsync(user);

    //    mockUserRepository.Verify(repo => repo.AddAsync(It.Is<User>(u => u.Password == "hashedpassword" && u.Role == UserRoles.User)), Times.Once);
    //}

    //[Fact]
    //public async Task LoginUserAsync_ShouldReturnUser_WhenCredentialsAreValid()
    //{
    //    var user = new User { Email = "validuser@example.com", Password = "hashedpassword" };
    //    mockUserRepository.Setup(repo => repo.FindByCondition(It.IsAny<Expression<Func<User, bool>>>())).Returns(new List<User> { user }.AsAsyncQueryable());

    //    var result = await userService.LoginUserAsync("validuser@example.com", "Password123");

    //    Assert.NotNull(result);
    //}

    [Fact]
    public async Task LoginUserAsync_ShouldReturnNull_WhenCredentialsAreInvalid()
    {
        mockUserRepository.Setup(repo => repo.FindByCondition(It.IsAny<Expression<Func<User, bool>>>())).Returns(new List<User>().AsAsyncQueryable());

        var result = await userService.LoginUserAsync("invaliduser@example.com", "Password123");

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldUpdateUser_WhenUserExists()
    {
        var user = new User { Id = 1, Username = "User1", Email = "user1@example.com" };
        mockUserRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(user);

        var updatedUser = new User { Id = 1, Username = "UpdatedUser", Email = "updateduser@example.com" };
        await userService.UpdateUserAsync(updatedUser);

        mockUserRepository.Verify(repo => repo.UpdateAsync(It.Is<User>(u => u.Username == "UpdatedUser" && u.Email == "updateduser@example.com")), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldDeleteUser_WhenUserExists()
    {
        var user = new User { Id = 1 };
        mockUserRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(user);

        await userService.DeleteUserAsync(1);

        mockUserRepository.Verify(repo => repo.DeleteAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        var expectedUser = new User { Id = 1 };
        mockUserRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(expectedUser);

        var result = await userService.GetUserByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnUser_WhenUserExists()
    {
        var expectedUser = new User { Email = "existinguser@example.com" };
        mockUserRepository.Setup(repo => repo.FindByCondition(It.IsAny<Expression<Func<User, bool>>>())).Returns(new List<User> { expectedUser }.AsAsyncQueryable());

        var result = await userService.GetUserByEmailAsync("existinguser@example.com");

        Assert.NotNull(result);
        Assert.Equal("existinguser@example.com", result.Email);
    }
}

