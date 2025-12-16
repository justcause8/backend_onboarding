using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entities.DbOnboarding;

public partial class QuestionOption
{
    public int Id { get; set; }

    public int FkQuestionId { get; set; }

    public string Text { get; set; } = null!;

    public bool CorrectAnswer { get; set; }

    public int OrderIndex { get; set; }

    public virtual Question FkQuestion { get; set; } = null!;
}
