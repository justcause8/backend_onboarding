using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entities.DbOnboardingRIMS;

public partial class Email
{
    public int Id { get; set; }

    public int FkUserId { get; set; }

    public string Email1 { get; set; } = null!;

    public virtual User FkUser { get; set; } = null!;
}
