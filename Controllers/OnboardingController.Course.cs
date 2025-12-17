using backend_onboarding.Models.DTOs;
using backend_onboarding.Services.Onboarding;
using Microsoft.AspNetCore.Mvc;

namespace backend_onboarding.Controllers
{
    public partial class OnboardingController : ControllerBase
    {
        [HttpGet("course/{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var course = await _onboardingService.GetCourseByIdAsync(id);

            if (course == null)
            {
                return NotFound(new { Message = $"Курс с ID {id} не найден" });
            }

            return Ok(course);
        }

        [HttpPost("course/create")]
        public async Task<IActionResult> Create([FromBody] CreateCourseRequest request)
        {
            try
            {
                var id = await _onboardingService.CreateCourseAsync(request);
                return Ok(new { Message = "Курс создан", CourseId = id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("course/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCourseRequest request)
        {
            var result = await _onboardingService.UpdateCourseAsync(id, request);
            if (!result) return NotFound("Курс не найден");
            return Ok(new { Message = "Курс обновлен" });
        }

        [HttpDelete("course/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _onboardingService.DeleteCourseAsync(id);
            if (!result) return NotFound("Курс не найден");
            return Ok(new { Message = "Курс и его материалы удалены" });
        }
    }
}
