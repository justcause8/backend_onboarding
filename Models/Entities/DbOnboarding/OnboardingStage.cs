using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entities.DbOnboarding;

public partial class OnboardingStage
{
    public int Id { get; set; }

    public int? FkOnboardingRouteId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int OrderIndex { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual OnboardingRoute? FkOnboardingRoute { get; set; }

    public virtual ICollection<UserOnboardingStageStatus> UserOnboardingStageStatuses { get; set; } = new List<UserOnboardingStageStatus>();
}
