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
    }
}