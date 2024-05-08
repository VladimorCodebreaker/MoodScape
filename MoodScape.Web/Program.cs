using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MoodScape.Data;
using MoodScape.Logic.IServices;
using MoodScape.Data.Repositories;
using MoodScape.Logic.Services;
using MoodScape.Data.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using MoodScape.Logic.Analysis;
using MoodScape.Logic.ConversationalAI;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add Configuration String
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));

// Add Services to the Container
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IMoodService, MoodService>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IHabitService, HabitService>();
builder.Services.AddScoped<IUserService, UserService>();

// Register SentimentAnalysisService
builder.Services.AddScoped<ISentimentAnalysisService>(serviceProvider =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    var apiKey = config.GetValue<string>("HuggingFaceAPI:ApiKey");
    return new SentimentAnalysisService(apiKey);
});

// Register ChatGPTService
builder.Services.AddScoped<IChatGPTService>(serviceProvider =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    return new ChatGPTService(config);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/User/Login";
    options.LogoutPath = "/User/Logout";
});

// Add PasswordHasher
builder.Services.AddScoped<PasswordHasher<User>>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed Database
DatabaseInitializer.SeedData(app.Services);

app.Run();

