using backend_onboarding.Services.Authentication;
using backend_onboarding.Services.Onboarding;
using Microsoft.AspNetCore.Mvc;

namespace backend_onboarding.Controllers
{
    [ApiController]
    [Route("onboarding")]
    public partial class OnboardingController : ControllerBase
    {
        private readonly IOnboardingService _onboardingService;

        public OnboardingController(IOnboardingService onboardingService)
        {
            _onboardingService = onboardingService;
        }

        protected int CurrentUserId => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                                      ?? User.FindFirst("UserId")?.Value
                                      ?? "0");

        protected ObjectResult Forbidden(string message)
        {
            return StatusCode(403, new { error = "Forbidden", message });
        }

        protected bool IsHr => User.IsInRole(OnboardingRoles.HrAdmin);
        protected bool IsMentor => User.IsInRole(OnboardingRoles.Mentor);

        protected IActionResult EnsureHrAdmin()
        {
            if (!IsHr)
            {
                return Forbidden("У вас недостаточно прав.");
            }
            return null;
        }

        protected IActionResult ProcessResult(bool success, string errorMessage, string successMessage = "Операция выполнена")
        {
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { Message = successMessage });
        }

        // Универсальная проверка прав (HR или Ментор)
        protected bool IsHrOrMentor => IsHr || IsMentor;

        protected IActionResult EnsureHrOrMentor()
        {
            if (!IsHrOrMentor) return Forbidden("Доступ разрешен только HR или Наставникам.");
            return null;
        }
    }
}