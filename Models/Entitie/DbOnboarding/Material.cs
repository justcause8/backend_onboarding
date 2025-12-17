using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entitie.DbOnboarding;

public partial class Material
{
    public int Id { get; set; }

    public int FkCourseId { get; set; }

    public string? UrlDocument { get; set; }

    public virtual Course FkCourse { get; set; } = null!;
}
