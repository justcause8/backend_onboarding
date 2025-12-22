using backend_onboarding.Models.DTOs;
using backend_onboarding.Services.Authentication;
using backend_onboarding.Services.Onboarding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_onboarding.Controllers
{
    public partial class OnboardingController : ControllerBase
    {
        [HttpGet("course/{id}")]
        [Authorize]
        public async Task<IActionResult> GetCourse(int id)
        {
            var course = await _onboardingService.GetCourseByIdAsync(id);

            if (course == null)
            {
                return NotFound(new { Message = $"Курс с ID {id} не найден" });
            }

            return Ok(course);
        }

        [HttpGet("courses")]
        [Authorize]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _onboardingService.GetAllCoursesFullAsync();
            return Ok(courses);
        }

        [HttpPost("course/create")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> Create([FromBody] CreateCourseRequest request)
        {
            if (request == null) return BadRequest("Данные не заполнены");

            request.AuthorId = CurrentUserId;

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
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseRequest request)
        {
            var existingCourse = await _onboardingService.GetCourseByIdAsync(id);
            if (existingCourse == null) return NotFound("Курс не найден");

            // Проверка прав: только автор или HR-админ (IsHr мы определили ранее в partial классе)
            if (!IsHr && existingCourse.AuthorId != CurrentUserId)
            {
                return Forbidden("Вы можете редактировать только те курсы, автором которых являетесь.");
            }

            // Вызываем сервис с новым типом DTO
            var result = await _onboardingService.UpdateCourseAsync(id, request);

            if (!result) return BadRequest(new { Message = "Не удалось сохранить изменения в базе данных" });

            return Ok(new { Message = "Курс успешно обновлен" });
        }

        [HttpDelete("course/{id}")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> Delete(int id)
        {
            var existingCourse = await _onboardingService.GetCourseByIdAsync(id);
            if (existingCourse == null) return NotFound("Курс не найден");

            if (existingCourse.AuthorId != CurrentUserId)
            {
                return Forbid("Удаление чужого курса запрещено");
            }

            await _onboardingService.DeleteCourseAsync(id);
            return Ok(new { Message = "Курс удален" });
        }

        [HttpGet("course/user-progress")]
        [Authorize]
        public async Task<IActionResult> GetUserCourseProgress()
        {
            var userId = CurrentUserId; // предполагается, что у вас есть свойство в базовом контроллере
            var progress = await _onboardingService.GetUserCourseProgressAsync(userId);
            return Ok(progress);
        }
    }
}
