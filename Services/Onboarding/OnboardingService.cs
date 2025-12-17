using backend_onboarding.Models.Entitie.DbOnboarding;
using backend_onboarding.Models.Entitie.DbOnboardingRIMS;

namespace backend_onboarding.Services.Onboarding
{
    public partial class OnboardingService : IOnboardingService
    {
        private readonly OnboardingContext _onboardingContext;

        public OnboardingService(OnboardingRimsContext rimsContext, OnboardingContext onboardingContext)
        {
            _onboardingContext = onboardingContext;
        }
    }
}
