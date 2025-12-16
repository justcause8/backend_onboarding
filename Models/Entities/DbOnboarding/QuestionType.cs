using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entities.DbOnboarding;

public partial class QuestionType
{
    public int Id { get; set; }

    public string NameQuestionType { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
