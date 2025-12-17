using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entitie.DbOnboarding;

public partial class Test
{
    public int Id { get; set; }

    public int? Fk1CourseId { get; set; }

    public int Fk2UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? PassingScore { get; set; }

    public decimal? ResultsScore { get; set; }

    public string Status { get; set; } = null!;

    public virtual Course? Fk1Course { get; set; }

    public virtual User Fk2User { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
