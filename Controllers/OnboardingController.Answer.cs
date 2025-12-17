using backend_onboarding.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace backend_onboarding.Controllers
{
    public partial class OnboardingController : ControllerBase
    {
        [HttpPost("answer")]
        public async Task<IActionResult> CreateAnswer([FromBody] UserAnswerRequest request)
        {
            var id = await _onboardingService.SubmitAnswerAsync(request);
            return Ok(new { Message = "Ответ сохранен", AnswerId = id });
        }

        [HttpPut("answer/{id}")]
        public async Task<IActionResult> UpdateAnswer(int id, [FromBody] UserAnswerRequest request)
        {
            var success = await _onboardingService.UpdateAnswerAsync(id, request);
            if (!success) return NotFound("Ответ не найден");
            return Ok(new { Message = "Ответ обновлен" });
        }

        [HttpDelete("answer/{id}")]
        public async Task<IActionResult> DeleteAnswer(int id)
        {
            var success = await _onboardingService.DeleteAnswerAsync(id);
            if (!success) return NotFound("Ответ не найден");
            return Ok(new { Message = "Ответ удален" });
        }

        [HttpGet("user/{userId}/test/{testId}/answers")]
        public async Task<IActionResult> GetAnswers(int userId, int testId)
        {
            var answers = await _onboardingService.GetUserAnswersByTestAsync(userId, testId);
            return Ok(answers);
        }
    }
}
