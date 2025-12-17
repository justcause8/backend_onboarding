using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entitie.DbOnboarding;

public partial class Answer
{
    public int Id { get; set; }

    public int Fk1UserId { get; set; }

    public int Fk2QuestionId { get; set; }

    public string AnswerText { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();

    public virtual User Fk1User { get; set; } = null!;

    public virtual Question Fk2Question { get; set; } = null!;
}
