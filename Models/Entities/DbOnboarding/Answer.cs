using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entities.DbOnboarding;

public partial class Answer
{
    public int Id { get; set; }

    public int FkUserId { get; set; }

    public int FkQuestionId { get; set; }

    public string AnswerText { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();

    public virtual Question FkQuestion { get; set; } = null!;

    public virtual User FkUser { get; set; } = null!;
}
