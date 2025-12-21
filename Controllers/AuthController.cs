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

        [HttpGet("whoami")]
        [Authorize(AuthenticationSchemes = "Cookies,Negotiate")]
        public async Task<IActionResult> WhoAmI()
        {
            if (User.Identity != null &&
                User.Identity.IsAuthenticated &&
                User.Identity.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme)
            {
                return Ok(new
                {
                    Name = User.FindFirst("FullName")?.Value ?? User.Identity.Name,
                    Login = User.Identity.Name,
                    Role = User.FindFirst(ClaimTypes.Role)?.Value,
                    Source = "Cookie"
                });
            }

            var user = await _authService.AuthenticateAsync();

            if (user == null)
                return Unauthorized(new { message = "Пользователь не найден в БД Onboarding/RIMS" });

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString()),
                new Claim("FullName", user.Name)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = true });

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

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Вышли из системы" });
        }
    }
}