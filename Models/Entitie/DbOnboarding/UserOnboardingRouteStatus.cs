using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entitie.DbOnboarding;

public partial class UserOnboardingRouteStatus
{
    public int Id { get; set; }

    public int FkUserId { get; set; }

    public int FkOnboardingRouteId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? FactStartDate { get; set; }

    public DateTime? FactEndDate { get; set; }

    public virtual OnboardingRoute FkOnboardingRoute { get; set; } = null!;

    public virtual User FkUser { get; set; } = null!;
}
