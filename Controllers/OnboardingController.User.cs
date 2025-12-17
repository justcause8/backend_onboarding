using backend_onboarding.Models.DTOs;
using backend_onboarding.Services.Onboarding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_onboarding.Controllers
{
    public partial class OnboardingController : ControllerBase
    {
        [HttpGet("users")]
         [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _onboardingService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("user/{userId}")]
        // [Authorize]
        public async Task<IActionResult> GetUserInfo(int userId)
        {
            var response = await _onboardingService.GetUserOnboardingDataAsync(userId);

            if (response == null)
            {
                return NotFound(new { message = $"Пользователь с ID {userId} не найден" });
            }

            return Ok(response);
        }

        // Обновление
        [HttpPut("user/{userId}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateOnboardingUserRequest request)
        {
            // Вызываем метод из OnboardingService
            var success = await _onboardingService.UpdateOnboardingUserAsync(userId, request);

            if (!success)
            {
                return NotFound(new { message = $"Пользователь с ID {userId} не найден" });
            }

            return Ok(new { message = "Данные пользователя успешно обновлены" });
        }

        // Удаление
        [HttpDelete("user/{userId}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var success = await _onboardingService.DeleteOnboardingUserAsync(userId);

            if (!success)
            {
                return NotFound(new { message = $"Пользователь с ID {userId} не найден" });
            }

            return Ok(new { message = "Пользователь успешно удален" });
        }
    }
}
