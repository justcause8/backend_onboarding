using System;
using System.Collections.Generic;

namespace backend_onboarding.Models.Entities.DbOnboardingRIMS;

public partial class User
{
    public int Id { get; set; }

    public Guid Uid { get; set; }

    public string Caption { get; set; } = null!;

    public string Company { get; set; } = null!;

    public string Department { get; set; } = null!;

    public string JobTitle { get; set; } = null!;

    public virtual ICollection<Adaccount> Adaccounts { get; set; } = new List<Adaccount>();

    public virtual ICollection<Email> Emails { get; set; } = new List<Email>();
}
