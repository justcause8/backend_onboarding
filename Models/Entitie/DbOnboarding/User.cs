using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entitie.DbOnboarding;

public partial class User
{
    public int Id { get; set; }

    public Guid? Uid { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Department { get; set; } = null!;

    public string JobTitle { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? Login { get; set; }

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual ICollection<OnboardingRoute> OnboardingRoutes { get; set; } = new List<OnboardingRoute>();

    public virtual ICollection<SystemAudit> SystemAudits { get; set; } = new List<SystemAudit>();

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();

    public virtual ICollection<UserOnboardingRouteStatus> UserOnboardingRouteStatuses { get; set; } = new List<UserOnboardingRouteStatus>();

    public virtual ICollection<UserOnboardingStageStatus> UserOnboardingStageStatuses { get; set; } = new List<UserOnboardingStageStatus>();

    public virtual ICollection<UserProgress> UserProgresses { get; set; } = new List<UserProgress>();
}
