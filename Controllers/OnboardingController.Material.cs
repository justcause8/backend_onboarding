using backend_onboarding.Models.DTOs;
using backend_onboarding.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_onboarding.Controllers
{
    public partial class OnboardingController : ControllerBase
    {
        // Получить все материалы конкретного курса
        [HttpGet("course/{courseId}/materials")]
        [Authorize]
        public async Task<IActionResult> GetMaterialsByCourse(int courseId)
        {
            var materials = await _onboardingService.GetMaterialsByCourseIdAsync(courseId);
            return Ok(materials);
        }

        // Получить один материал по ID
        [HttpGet("material/{id}")]
        [Authorize]
        public async Task<IActionResult> GetMaterial(int id)
        {
            var material = await _onboardingService.GetMaterialByIdAsync(id);
            if (material == null) return NotFound("Материал не найден");
            return Ok(material);
        }

        // Создать материал
        [HttpPost("material")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> CreateMaterial([FromBody] CreateMaterialRequest request)
        {
            try
            {
                var id = await _onboardingService.CreateMaterialAsync(request);
                return Ok(new { Message = "Материал добавлен", MaterialId = id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Редактировать материал
        [HttpPut("material/{id}")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> UpdateMaterial(int id, [FromBody] UpdateMaterialRequest request)
        {
            var success = await _onboardingService.UpdateMaterialAsync(id, request);
            if (!success) return NotFound("Материал не найден");
            return Ok(new { Message = "Материал обновлен" });
        }

        // Удалить материал
        [HttpDelete("material/{id}")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> DeleteMaterial(int id)
        {
            var success = await _onboardingService.DeleteMaterialAsync(id);
            if (!success) return NotFound("Материал не найден");
            return Ok(new { Message = "Материал удален" });
        }
    }
}
