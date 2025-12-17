using backend_onboarding.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace backend_onboarding.Controllers
{
    public partial class OnboardingController : ControllerBase
    {
        [HttpGet("course/{courseId}/tests")]
        public async Task<IActionResult> GetTestsByCourse(int courseId)
        {
            var tests = await _onboardingService.GetTestsByCourseIdAsync(courseId);
            return Ok(tests);
        }

        [HttpGet("test/{id}")]
        public async Task<IActionResult> GetTest(int id, [FromQuery] int userId)
        {
            var test = await _onboardingService.GetTestByIdAsync(id, userId);
            if (test == null) return NotFound("Тест не найден");
            return Ok(test);
        }

        [HttpPost("test")]
        public async Task<IActionResult> CreateTest([FromBody] CreateTestRequest request)
        {
            var id = await _onboardingService.CreateTestAsync(request);
            return Ok(new { Message = "Тест создан", TestId = id });
        }

        [HttpPut("test/{id}")]
        public async Task<IActionResult> UpdateTest(int id, [FromBody] CreateTestRequest request)
        {
            var success = await _onboardingService.UpdateTestAsync(id, request);
            if (!success) return NotFound("Тест не найден");
            return Ok(new { Message = "Тест обновлен" });
        }

        [HttpDelete("test/{id}")]
        public async Task<IActionResult> DeleteTest(int id)
        {
            var success = await _onboardingService.DeleteTestAsync(id);
            if (!success) return NotFound("Тест не найден");
            return Ok(new { Message = "Тест удален" });
        }
    }
}
