using backend_onboarding.Models.DTOs;
using backend_onboarding.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_onboarding.Controllers
{
    public partial class OnboardingController : ControllerBase
    {
        [HttpGet("course/{courseId}/tests")]
        [Authorize]
        public async Task<IActionResult> GetTestsByCourse(int courseId)
        {
            var tests = await _onboardingService.GetTestsByCourseIdAsync(courseId);
            return Ok(tests);
        }

        [HttpGet("test/{id}")]
        [Authorize]
        public async Task<IActionResult> GetTest(int id)
        {
            var test = await _onboardingService.GetTestByIdAsync(id, CurrentUserId);

            if (test == null) return NotFound("Тест не найден");
            return Ok(test);
        }

        [HttpPost("test")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> CreateTest([FromBody] CreateTestRequest request)
        {
            if (request == null) return BadRequest("Данные не заполнены");

            request.AuthorId = CurrentUserId;

            var id = await _onboardingService.CreateTestAsync(request);
            return Ok(new { Message = "Тест создан", TestId = id });
        }

        [HttpPut("test/{id}")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> UpdateTest(int id, [FromBody] CreateTestRequest request)
        {
            var existingTest = await _onboardingService.GetTestByIdAsync(id, CurrentUserId);

            if (existingTest == null) return NotFound("Тест не найден");

            if (existingTest.AuthorId != CurrentUserId)
            {
                return Forbid("Вы не являетесь автором этого теста и не можете его изменять");
            }

            request.AuthorId = existingTest.AuthorId;

            var success = await _onboardingService.UpdateTestAsync(id, request);
            if (!success) return BadRequest("Ошибка при обновлении теста");

            return Ok(new { Message = "Тест обновлен" });
        }

        [HttpDelete("test/{id}")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> DeleteTest(int id)
        {
            var existingTest = await _onboardingService.GetTestByIdAsync(id, CurrentUserId);

            if (existingTest == null) return NotFound("Тест не найден");

            if (existingTest.AuthorId != CurrentUserId)
            {
                return Forbid("Удаление чужого теста запрещено");
            }

            var success = await _onboardingService.DeleteTestAsync(id);
            if (!success) return BadRequest("Не удалось удалить тест");

            return Ok(new { Message = "Тест удален" });
        }
    }
}
