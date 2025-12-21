using backend_onboarding.Models.Entitie.DbOnboarding;
using backend_onboarding.Models.Entitie.DbOnboardingRIMS;
using backend_onboarding.Services.Authentication;
using backend_onboarding.Services.Onboarding;
using backend_onboarding.Services.Rims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Добавление DbContext
builder.Services.AddDbContext<OnboardingRimsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RimsConnection")));

builder.Services.AddDbContext<OnboardingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OnboardingConnection")));

//Регистрация сервисов
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRimsService, RimsService>();
builder.Services.AddScoped<IOnboardingService, OnboardingService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = "Onboarding.Auth";
    options.Cookie.HttpOnly = true;

    // Настройки для Cross-Origin (Frontend 5173 -> API 7054)
    options.Cookie.SameSite = SameSiteMode.None; // Важно для Cross-Origin
    //options.Cookie.SameSite = SameSiteMode.Lax; // Для HTTP
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    //options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Для HTTP

    // ПРЕДОТВРАЩЕНИЕ РЕДИРЕКТОВ
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };

    // Настройка времени жизни куки
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
})
.AddNegotiate(); // Добавляем Windows Auth


// Настраиваем авторизацию так, чтобы она принимала ЛЮБУЮ из схем
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme, NegotiateDefaults.AuthenticationScheme)
        .Build();
});

builder.Services.AddControllers(); // Добавляем контроллеры для API

var app = builder.Build(); // Создание приложения

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Генерация спецификации Swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseCors(policy => policy
    .WithOrigins("http://localhost:5173")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()); // Обязательно для передачи кук!


// Middleware pipeline
app.UseHttpsRedirection(); // Перенаправляет HTTP на HTTPS
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Маппинг контроллеров
app.MapControllers();
app.Run();
