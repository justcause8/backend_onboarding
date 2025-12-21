using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entitie.DbOnboarding;

public partial class Course
{
    public int Id { get; set; }

    public int? FkOnbordingStage { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int OrderIndex { get; set; }

    public string Status { get; set; } = null!;

    public virtual OnboardingStage? FkOnbordingStageNavigation { get; set; }

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();

    public virtual ICollection<UserProgress> UserProgresses { get; set; } = new List<UserProgress>();
}
