using backend_onboarding.Models.DTOs;
using backend_onboarding.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_onboarding.Controllers
{
    public partial class OnboardingController : ControllerBase
    {
        [HttpGet("test/{testId}/questions")]
        [Authorize]
        public async Task<IActionResult> GetQuestions(int testId)
        {
            var questions = await _onboardingService.GetQuestionsByTestIdAsync(testId);
            return Ok(questions);
        }

        [HttpPost("question")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionRequest request)
        {
            var id = await _onboardingService.CreateQuestionAsync(request);
            return Ok(new { Message = "Вопрос создан", QuestionId = id });
        }

        [HttpPut("question/{id}")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] CreateQuestionRequest request)
        {
            var success = await _onboardingService.UpdateQuestionAsync(id, request);
            if (!success) return NotFound("Вопрос не найден");
            return Ok(new { Message = "Вопрос обновлен" });
        }

        [HttpDelete("question/{id}")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var success = await _onboardingService.DeleteQuestionAsync(id);
            if (!success) return NotFound("Вопрос не найден");
            return Ok(new { Message = "Вопрос удален" });
        }
    }
}
