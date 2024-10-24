using System;
using System.Collections.Generic;

namespace ScaffoldDB.Migrations;

public partial class Rent
{
    public Guid Id { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal Cost { get; set; }

    public string Status { get; set; } = null!;

    public string? Description { get; set; }

    public string VehicleLicensePlate { get; set; } = null!;

    public string ClientEmail { get; set; } = null!;

    public Guid WorkerId { get; set; }

    public virtual Client ClientEmailNavigation { get; set; } = null!;

    public virtual ICollection<DamageReport> DamageReports { get; set; } = new List<DamageReport>();

    public virtual Invoice? Invoice { get; set; }

    public virtual Vehicle VehicleLicensePlateNavigation { get; set; } = null!;

    public virtual Worker Worker { get; set; } = null!;
}
