using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entities.DbOnboarding;

public partial class UserOnboardingStageStatus
{
    public int Id { get; set; }

    public int FkUserId { get; set; }

    public int FkOnboardingStageId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? FactStartDate { get; set; }

    public DateTime? FactEndDate { get; set; }

    public virtual OnboardingStage FkOnboardingStage { get; set; } = null!;

    public virtual User FkUser { get; set; } = null!;
}
