using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entitie.DbOnboarding;

public partial class Test
{
    public int Id { get; set; }

    public int FkCourseId { get; set; }

    public int FkUserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? PassingScore { get; set; }

    public decimal? ResultsScore { get; set; }

    public string Status { get; set; } = null!;

    public virtual Course FkCourse { get; set; } = null!;

    public virtual User FkUser { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
