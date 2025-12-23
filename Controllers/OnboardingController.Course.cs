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

            if (!IsHr && existingCourse.AuthorId != CurrentUserId)
            {
                return Forbidden("Вы можете редактировать только те курсы, автором которых являетесь.");
            }

            var result = await _onboardingService.UpdateCourseAsync(id, request);

            if (!result) return BadRequest(new { Message = "Не удалось сохранить изменения в базе данных" });

            return Ok(new { Message = "Курс успешно обновлен" });
        }

        [HttpDelete("course/{id}")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> Delete(int id)
        {
            var existingCourse = await _onboardingService.GetCourseByIdAsync(id);
            if (existingCourse == null) return NotFound(new { Message = "Курс не найден" });

            if (existingCourse.AuthorId != CurrentUserId)
            {
                return StatusCode(403, new
                {
                    error = "Forbidden",
                    message = "Удаление чужого курса запрещено"
                });
            }

            var result = await _onboardingService.DeleteCourseAsync(id);

            if (!result)
                return BadRequest(new { Message = "Не удалось удалить курс" });

            return Ok(new { Message = "Курс удален" });
        }

        [HttpPost("course/{courseId}/start")]
        [Authorize]
        public async Task<IActionResult> StartCourse(int courseId)
        {
            var userId = CurrentUserId;

            try
            {
                var result = await _onboardingService.StartCourseAsync(userId, courseId);

                if (result)
                {
                    return Ok(new
                    {
                        Message = "Курс начат",
                        CourseId = courseId,
                        Status = "success",
                        Timestamp = DateTime.UtcNow
                    });
                }
                else
                {
                    // Возвращаем 200 OK даже если курс уже был начат
                    // Или если SaveChanges вернул 0 из-за отсутствия изменений
                    return Ok(new
                    {
                        Message = "Курс уже начат или не требует изменений",
                        CourseId = courseId,
                        Status = "info",
                        Timestamp = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                Console.WriteLine($"Ошибка StartCourse: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                // Но возвращаем 200 чтобы пользователь мог продолжить
                return Ok(new
                {
                    Message = "Произошла ошибка, но вы можете продолжить изучение курса",
                    CourseId = courseId,
                    Status = "warning",
                    Error = ex.Message
                });
            }
        }

        // Завершить курс (автоматически после теста)
        [HttpPost("course/{courseId}/complete")]
        [Authorize]
        public async Task<IActionResult> CompleteCourse(int courseId)
        {
            var userId = CurrentUserId;
            var result = await _onboardingService.CompleteCourseAsync(userId, courseId);

            return result
                ? Ok(new { Message = "Курс завершен", CourseId = courseId })
                : BadRequest(new { Message = "Курс еще не завершен или не найден" });
        }

        [HttpPost("course/{courseId}/check-completion")]
        [Authorize]
        public async Task<IActionResult> CheckCourseCompletion(int courseId)
        {
            var userId = CurrentUserId;

            try
            {
                var result = await _onboardingService.CheckAndUpdateCourseCompletionAsync(userId, courseId);

                if (result)
                    return Ok(new
                    {
                        Message = "Курс завершен",
                        CourseId = courseId,
                        IsCompleted = true
                    });
                else
                    return Ok(new
                    {
                        Message = "Курс еще не завершен",
                        CourseId = courseId,
                        IsCompleted = false
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Ошибка: {ex.Message}" });
            }
        }
    }
}
