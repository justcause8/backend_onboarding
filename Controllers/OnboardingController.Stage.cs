using backend_onboarding.Models.DTOs;
using backend_onboarding.Services.Authentication;
using backend_onboarding.Services.Onboarding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_onboarding.Controllers
{
    public partial class OnboardingController : ControllerBase
    {
        [HttpPost("stage/assign")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> AssignToStage([FromBody] AssignUserToStageRequest request)
        {
            var success = await _onboardingService.AssignUserToStageAsync(request.UserId, request.StageId);
            return ProcessResult(success, "Не удалось назначить этап пользователю", "Пользователь назначен на этап");
        }

        [HttpPost("route/assign-to-user")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> AssignRouteToUser([FromBody] AssignUserToRouteRequest request)
        {
            var success = await _onboardingService.AssignUserToRouteAsync(request.UserId, request.RouteId);
            return ProcessResult(success, "Маршрут не найден или не содержит этапов", "Маршрут назначен пользователю");
        }

        [HttpGet("stage/{id}")]
        [Authorize]
        public async Task<IActionResult> GetStage(int id)
        {
            var stage = await _onboardingService.GetStageByIdAsync(id);
            if (stage == null) return NotFound(new { Message = $"Этап с ID {id} не найден" });
            return Ok(stage);
        }

        [HttpPost("route/add-stages")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> AddStages([FromBody] AddStagesToRouteRequest request)
        {
            if (request == null || !request.Stages.Any())
                return BadRequest("Данные не заполнены");

            var success = await _onboardingService.AddStagesToRouteAsync(request);

            return ProcessResult(success, "Маршрут не найден", "Этапы добавлены и назначены пользователям");
        }

        [HttpPut("stage/{id}")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> UpdateStage(int id, [FromBody] UpdateStageRequest request)
        {
            var success = await _onboardingService.UpdateStageAsync(id, request);
            return ProcessResult(success, "Этап не найден", "Этап обновлен");
        }

        [HttpDelete("stage/{id}")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> DeleteStage(int id)
        {
            var success = await _onboardingService.DeleteStageAsync(id);
            return ProcessResult(success, "Этап не найден", "Этап удален");
        }
    }
}
