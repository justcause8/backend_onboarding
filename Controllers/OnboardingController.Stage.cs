using backend_onboarding.Models.DTOs;
using backend_onboarding.Services.Authentication;
using backend_onboarding.Services.Onboarding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_onboarding.Controllers
{
    public partial class OnboardingController : ControllerBase
    {
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

        [HttpPost("stage/assign")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> AssignToStage([FromBody] AssignUserToStageRequest request)
        {
            var success = await _onboardingService.AssignUserToStageAsync(request.UserId, request.StageId);
            return ProcessResult(success, "Не удалось назначить этап пользователю", "Пользователь назначен на этап");
        }

        [HttpPut("stage/{stageId}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateStageStatus(int stageId, [FromBody] UpdateStageStatusRequest request)
        {
            var userId = CurrentUserId;

            try
            {
                var result = await _onboardingService.UpdateStageStatusAsync(userId, stageId, request.Status);

                if (result)
                    return Ok(new { Message = "Статус этапа обновлен" });
                else
                    return BadRequest(new { Message = "Не удалось обновить статус" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Ошибка: {ex.Message}" });
            }
        }


        //[HttpPost("recalculate-stage-statuses")]
        //[Authorize]
        //public async Task<IActionResult> RecalculateStageStatuses()
        //{
        //    var userId = CurrentUserId;

        //    try
        //    {
        //        await _onboardingService.RecalculateStageStatusesAsync(userId);
        //        return Ok(new { Message = "Статусы этапов обновлены" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Message = $"Ошибка: {ex.Message}" });
        //    }
        //}
    }
}
