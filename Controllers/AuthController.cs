using backend_onboarding.Models.DTOs;
using backend_onboarding.Services.Authentication;
using backend_onboarding.Services.Onboarding;
using backend_onboarding.Services.Rims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace backend_onboarding.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _environment;

        public AuthController(IAuthService authService, IWebHostEnvironment environment)
        {
            _authService = authService;
            _environment = environment;
        }

        // --- АУТЕНТИФИКАЦИЯ --

        [HttpGet("whoami")]
        // Убираем [Authorize], чтобы метод срабатывал и для тех, у кого нет куки (попытка WinAuth)
        public async Task<IActionResult> WhoAmI()
        {
            // 1. СНАЧАЛА проверяем Куку (Cookie)
            // Если пользователь уже залогинился (например, через login-as), то User.Identity.IsAuthenticated будет true
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Достаем данные прямо из "кармана" (Claims), не обращаясь к базе данных
                return Ok(new
                {
                    Name = User.FindFirst("FullName")?.Value ?? User.Identity.Name,
                    Login = User.Identity.Name,
                    Role = User.FindFirst(ClaimTypes.Role)?.Value,
                    Source = "Cookie" // Для отладки, чтобы видеть, откуда пришли данные
                });
            }

            // 2. Если Куки нет, пробуем Windows Auth (твоя старая логика)
            var user = await _authService.AuthenticateAsync();

            if (user == null)
                return Unauthorized(new { message = "Учётная запись не зарегистрирована в корпоративной системе (Windows Auth failed)" });

            // Если Windows Auth прошла успешно, создаем куку (как мы делали раньше)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString()),
                new Claim("FullName", user.Name)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Ok(new
            {
                user.Name,
                user.Login,
                user.Role,
                Source = "WindowsAuth"
            });
        }

        [HttpPost("login-as")]
        public async Task<IActionResult> LoginAs([FromQuery] string login)
        {
            if (!_environment.IsDevelopment())
            {
                return Forbid();
            }

            var user = await _authService.AuthenticateByLoginAsync(login);

            if (user == null)
                return Unauthorized(new { message = $"Пользователь {login} не найден в системе" });

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString()),
                new Claim("FullName", user.Name)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Ok(new { user.Name, user.Login, user.Role, message = $"Успешный вход как {login}" });
        }

        // Опционально: Метод выхода
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Вышли из системы" });
        }
    }
}