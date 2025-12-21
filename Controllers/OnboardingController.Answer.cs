using backend_onboarding.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_onboarding.Controllers
{
    public partial class OnboardingController : ControllerBase
    {
        [HttpGet("user/{userId}/test/{testId}/answers")]
        [Authorize]
        public async Task<IActionResult> GetAnswers(int userId, int testId)
        {
            var answers = await _onboardingService.GetUserAnswersByTestAsync(userId, testId);
            return Ok(answers);
        }

        [HttpPost("answer")]
        [Authorize]
        public async Task<IActionResult> CreateAnswer([FromBody] UserAnswerRequest request)
        {
            request.UserId = CurrentUserId;

            var id = await _onboardingService.SubmitAnswerAsync(request);
            return Ok(new { Message = "Ответ сохранен", AnswerId = id });
        }

        [HttpPut("answer/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAnswer(int id, [FromBody] UserAnswerRequest request)
        {
            var success = await _onboardingService.UpdateAnswerAsync(id, request, CurrentUserId);

            if (!success) return Forbid("Вы не можете редактировать чужой ответ или ответ не найден");

            return Ok(new { Message = "Ответ обновлен" });
        }

        [HttpDelete("answer/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAnswer(int id)
        {
            var success = await _onboardingService.DeleteAnswerAsync(id, CurrentUserId);

            if (!success) return Forbid("Вы не можете удалить чужой ответ или ответ не найден");

            return Ok(new { Message = "Ответ удален" });
        }
    }
}
