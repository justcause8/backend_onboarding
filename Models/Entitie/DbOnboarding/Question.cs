using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entitie.DbOnboarding;

public partial class Question
{
    public int Id { get; set; }

    public int Fk1TestId { get; set; }

    public int Fk2QuestionTypeId { get; set; }

    public string TextQuestion { get; set; } = null!;

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual Test Fk1Test { get; set; } = null!;

    public virtual QuestionType Fk2QuestionType { get; set; } = null!;

    public virtual ICollection<QuestionOption> QuestionOptions { get; set; } = new List<QuestionOption>();
}
