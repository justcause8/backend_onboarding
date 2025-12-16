using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entities.DbOnboarding;

public partial class AnswerOption
{
    public int Id { get; set; }

    public int FkAnswerId { get; set; }

    public int? SelectedAnswerOption { get; set; }

    public virtual Answer FkAnswer { get; set; } = null!;
}
