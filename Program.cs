using backend_onboarding.Models.Entities.DbOnboarding;
using backend_onboarding.Models.Entities.DbOnboardingRIMS;
using backend_onboarding.Services.Authentication;
using backend_onboarding.Services.Onboarding;
using backend_onboarding.Services.Rims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Добавление DbContext
builder.Services.AddDbContext<OnboardingRimsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RimsConnection")));

builder.Services.AddDbContext<OnboardingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OnboardingConnection")));

// Регистрация сервисов
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRimsService, RimsService>();
builder.Services.AddScoped<IOnboardingService, OnboardingService>();

// Настройка аутентификации через cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization(); // Регстрация сервисов авторизации

builder.Services.AddControllers(); // Добавляем контроллеры для API

var app = builder.Build(); // Создание приложения

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Генерация спецификации Swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"); // Отображение UI с конечной точкой Swagger
        options.RoutePrefix = string.Empty; // Открытие UI по корневому адресу (например, http://localhost:5000)
    });
}

// Middleware pipeline
app.UseHttpsRedirection(); // Перенаправляет HTTP на HTTPS
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Маппинг контроллеров
app.MapControllers();
app.Run();
