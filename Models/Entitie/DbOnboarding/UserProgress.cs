using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entitie.DbOnboarding;

public partial class UserProgress
{
    public int Id { get; set; }

    public int FkUserId { get; set; }

    public int FkCourseId { get; set; }

    public string? Status { get; set; }

    public virtual Course FkCourse { get; set; } = null!;

    public virtual User FkUser { get; set; } = null!;
}
