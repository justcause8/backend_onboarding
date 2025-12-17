using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entitie.DbOnboarding;

public partial class OnboardingRoute
{
    public int Id { get; set; }

    public int? FkmentorId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual User? Fkmentor { get; set; }

    public virtual ICollection<OnboardingStage> OnboardingStages { get; set; } = new List<OnboardingStage>();

    public virtual ICollection<UserOnboardingRouteStatus> UserOnboardingRouteStatuses { get; set; } = new List<UserOnboardingRouteStatus>();
}
