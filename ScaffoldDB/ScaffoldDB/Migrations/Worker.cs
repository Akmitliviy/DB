using System;
using System.Collections.Generic;

namespace ScaffoldDB.Migrations;

public partial class Worker
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string OccupationalPosition { get; set; } = null!;

    public string OfficeName { get; set; } = null!;

    public virtual Office OfficeNameNavigation { get; set; } = null!;

    public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();
}
