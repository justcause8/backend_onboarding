using backend_onboarding.Models.DTOs;
using backend_onboarding.Services.Onboarding;
using Microsoft.AspNetCore.Mvc;

namespace backend_onboarding.Controllers
{
    public partial class OnboardingController : ControllerBase
    {
        [HttpPost("stage/assign")]
        public async Task<IActionResult> AssignToStage([FromBody] AssignUserToStageRequest request)
        {
            var success = await _onboardingService.AssignUserToStageAsync(request.UserId, request.StageId);
            if (!success) return BadRequest("Не удалось назначить этап пользователю");
            return Ok(new { Message = "Пользователь назначен на этап, статус создан" });
        }

        [HttpPost("route/assign-to-user")]
        public async Task<IActionResult> AssignRouteToUser([FromBody] AssignUserToRouteRequest request)
        {
            var success = await _onboardingService.AssignUserToRouteAsync(request.UserId, request.RouteId);
            if (!success) return BadRequest("Маршрут не найден или не содержит этапов");
            return Ok(new { Message = "Маршрут назначен пользователю, все этапы добавлены в отслеживание" });
        }

        [HttpGet("stage/{id}")]
        public async Task<IActionResult> GetStage(int id)
        {
            var stage = await _onboardingService.GetStageByIdAsync(id);

            if (stage == null)
            {
                return NotFound(new { Message = $"Этап с ID {id} не найден" });
            }

            return Ok(stage);
        }

        // Добавление этапов к маршруту
        [HttpPost("stage/add")]
        public async Task<IActionResult> AddStages([FromBody] AddStagesToRouteRequest request)
        {
            var success = await _onboardingService.AddStagesToRouteAsync(request);
            if (!success) return NotFound("Маршрут не найден");
            return Ok(new { Message = "Этапы добавлены" });
        }

        // Редактирование одного этапа
        [HttpPut("stage/{id}")]
        public async Task<IActionResult> UpdateStage(int id, [FromBody] StageDto request)
        {
            var success = await _onboardingService.UpdateStageAsync(id, request);
            if (!success) return NotFound("Этап не найден");
            return Ok(new { Message = "Этап обновлен" });
        }

        // Удаление одного этапа
        [HttpDelete("stage/{id}")]
        public async Task<IActionResult> DeleteStage(int id)
        {
            var success = await _onboardingService.DeleteStageAsync(id);
            if (!success) return NotFound("Этап не найден");
            return Ok(new { Message = "Этап удален" });
        }
    }
}
