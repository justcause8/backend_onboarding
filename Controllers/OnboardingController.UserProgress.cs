using backend_onboarding.Models.DTOs;
using backend_onboarding.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_onboarding.Controllers
{
    public partial class OnboardingController : ControllerBase
    {
        // Получение прогресса пользователя (упрощенная версия)
        [HttpGet("user-progress")]
        [Authorize]
        public async Task<IActionResult> GetUserProgress()
        {
            var userId = CurrentUserId;

            // Получаем прогресс - сервис уже должен возвращать корректный объект даже при отсутствии данных
            var progress = await _onboardingService.GetUserCourseProgressAsync(userId);

            // Если progress null (на всякий случай), возвращаем пустой объект
            return Ok(progress ?? new UserProgressResponse
            {
                TotalCourses = 0,
                CompletedCourses = 0,
                TotalStages = 0,
                CompletedStages = 0,
                StageProgress = new List<StageProgressItem>()
            });
        }

        // Ручной пересчет статусов этапов
        [HttpPost("recalculate-statuses")]
        [Authorize]
        public async Task<IActionResult> RecalculateStatuses()
        {
            var userId = CurrentUserId;
            await _onboardingService.RecalculateStageStatusesAsync(userId);
            return Ok(new { Message = "Статусы этапов обновлены" });
        }
    }
}
