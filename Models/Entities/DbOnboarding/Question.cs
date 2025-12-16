using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entities.DbOnboarding;

public partial class Question
{
    public int Id { get; set; }

    public int FkTestId { get; set; }

    public int FkQuestionTypeId { get; set; }

    public string TextQuestion { get; set; } = null!;

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual QuestionType FkQuestionType { get; set; } = null!;

    public virtual Test FkTest { get; set; } = null!;

    public virtual ICollection<QuestionOption> QuestionOptions { get; set; } = new List<QuestionOption>();
}
