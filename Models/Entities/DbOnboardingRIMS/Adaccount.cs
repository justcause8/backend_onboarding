using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entities.DbOnboardingRIMS;

public partial class Adaccount
{
    public int Id { get; set; }

    public int FkUserId { get; set; }

    public string AccountName { get; set; } = null!;

    public string AppName { get; set; } = null!;

    public string Role { get; set; } = null!;

    public virtual User FkUser { get; set; } = null!;
}
