using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entitie.DbOnboarding;

public partial class SystemAudit
{
    public int Id { get; set; }

    public int? FkUserId { get; set; }

    public string Action { get; set; } = null!;

    public DateTime TimeInterval { get; set; }

    public virtual User? FkUser { get; set; }
}
