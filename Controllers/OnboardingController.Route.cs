using backend_onboarding.Models.DTOs;
using backend_onboarding.Services.Onboarding;
using Microsoft.AspNetCore.Mvc;

namespace backend_onboarding.Controllers
{
    public partial class OnboardingController : ControllerBase
    {
        [HttpGet("route/{id}")]
        public async Task<IActionResult> GetRouteById(int id)
        {
            var route = await _onboardingService.GetOnboardingRouteByIdAsync(id);

            if (route == null)
            {
                return NotFound(new { message = $"Маршрут с ID {id} не найден" });
            }

            return Ok(route);
        }

        [HttpPost("route/create")]
        public async Task<IActionResult> CreateRoute([FromBody] CreateOnboardingRouteRequest request)
        {
            if (request == null) return BadRequest("Данные не заполнены");

            try
            {
                var routeId = await _onboardingService.CreateOnboardingRouteAsync(request);
                return Ok(new { Message = "Маршрут успешно создан", RouteId = routeId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при создании маршрута: {ex.Message}");
            }
        }

        [HttpPut("route/{id}")]
        public async Task<IActionResult> UpdateRoute(int id, [FromBody] CreateOnboardingRouteRequest request)
        {
            var result = await _onboardingService.UpdateOnboardingRouteAsync(id, request);
            if (!result) return NotFound("Маршрут не найден");

            return Ok(new { Message = "Маршрут обновлен" });
        }

        [HttpDelete("route/{id}")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var result = await _onboardingService.DeleteOnboardingRouteAsync(id);
            if (!result) return NotFound("Маршрут не найден");

            return Ok(new { Message = "Маршрут удален" });
        }
    }
}
