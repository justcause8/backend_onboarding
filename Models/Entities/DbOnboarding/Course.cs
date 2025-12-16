using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entities.DbOnboarding;

public partial class Course
{
    public int Id { get; set; }

    public int? FkUserId { get; set; }

    public int? FkOnboardingStage { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int OrderIndex { get; set; }

    public string Status { get; set; } = null!;

    public virtual OnboardingStage? FkOnboardingStageNavigation { get; set; }

    public virtual User? FkUser { get; set; }

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
