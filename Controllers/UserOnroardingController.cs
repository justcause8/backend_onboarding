using backend_onboarding.Models.DTOs;
using backend_onboarding.Services.Onboarding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_onboarding.Controllers
{
    [Route("onboarding/user")]
    [ApiController]
    public class UserOnroardingController : ControllerBase
    {
        private readonly IOnboardingService _onboardingService;

        public UserOnroardingController(IOnboardingService onboardingService)
        {
            _onboardingService = onboardingService;
        }

        // --- ОБНОВЛЕНИЕ (ONBOARDING) ---

        [HttpPut("{userId}")]
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

        // --- УДАЛЕНИЕ (ONBOARDING) ---

        [HttpDelete("{userId}")]
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
