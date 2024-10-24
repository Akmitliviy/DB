using System;
using System.Collections.Generic;

namespace ScaffoldDB.Migrations;

public partial class InsurancePolicy
{
    public Guid Id { get; set; }

    public string PolicyNumber { get; set; } = null!;

    public string Provider { get; set; } = null!;

    public decimal Cost { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string VehicleLicensePlate { get; set; } = null!;

    public virtual Vehicle VehicleLicensePlateNavigation { get; set; } = null!;
}
