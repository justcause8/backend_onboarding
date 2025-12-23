using backend_onboarding.Models.DTOs;
using backend_onboarding.Models.Entitie.DbOnboarding;
using backend_onboarding.Services.Authentication;
using backend_onboarding.Services.Onboarding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_onboarding.Controllers
{
    public partial class OnboardingController
    {
        [HttpGet("route/{id}")]
        [Authorize]
        public async Task<IActionResult> GetRouteById(int id)
        {
            var route = await _onboardingService.GetOnboardingRouteByIdAsync(id);
            if (route == null) return NotFound(new { message = $"Маршрут с ID {id} не найден" });
            return Ok(route);
        }

        [HttpPost("route/create")]
        [Authorize(Roles = OnboardingRoles.HrAdmin)]
        public async Task<IActionResult> CreateRoute([FromBody] CreateOnboardingRouteRequest request)
        {
            var accessError = EnsureHrAdmin();
            if (accessError != null) return accessError;

            if (request == null) return BadRequest(new { message = "Данные запроса пусты" });

            request.MentorId ??= CurrentUserId;

            try
            {
                var routeId = await _onboardingService.CreateOnboardingRouteAsync(request);
                return Ok(new { Message = "Маршрут успешно создан", RouteId = routeId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при создании: {ex.Message}");
            }
        }

        [HttpPut("route/{id}")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> UpdateRoute(int id, [FromBody] UpdateOnboardingRouteRequest request)
        {
            var route = await _onboardingService.GetOnboardingRouteByIdAsync(id);
            if (route == null) return NotFound("Маршрут не найден");

            if (IsMentor && !IsHr && route.Mentor?.Id != CurrentUserId)
            {
                return Forbidden("Вы можете редактировать только свои маршруты.");
            }

            request.MentorId ??= route.Mentor?.Id;

            var result = await _onboardingService.UpdateOnboardingRouteAsync(id, request);
            return result ? Ok(new { Message = "Маршрут обновлен" }) : BadRequest(new { message = "Ошибка в БД" });
        }

        [HttpDelete("route/{id}")]
        [Authorize(Roles = OnboardingRoles.HrAdmin)]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var accessError = EnsureHrAdmin();
            if (accessError != null) return accessError;

            var result = await _onboardingService.DeleteOnboardingRouteAsync(id);
            return result ? Ok(new { Message = "Маршрут удален" }) : NotFound("Маршрут не найден");
        }

        [HttpGet("route/my-route")]
        [Authorize]
        public async Task<IActionResult> GetMyRouteId()
        {
            var routeId = await _onboardingService.GetMyRouteIdAsync(CurrentUserId);
            if (!routeId.HasValue)
            {
                return Ok(new { routeId = (int?)null });
            }
            return routeId.HasValue
                ? Ok(new { RouteId = routeId.Value })
                : Ok(new { Message = "Маршрут не назначен" });
        }

        [HttpPost("route/assign-to-user")]
        [Authorize(Roles = OnboardingRoles.HrAdmin + "," + OnboardingRoles.Mentor)]
        public async Task<IActionResult> AssignRouteToUser([FromBody] AssignUserToRouteRequest request)
        {
            var success = await _onboardingService.AssignUserToRouteAsync(request.UserId, request.RouteId);
            return ProcessResult(success, "Маршрут не найден или не содержит этапов", "Маршрут назначен пользователю");
        }
    }
}
