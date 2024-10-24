using System;
using System.Collections.Generic;

namespace ScaffoldDB.Migrations;

public partial class ServiceRecord
{
    public Guid Id { get; set; }

    public DateOnly ServiceDate { get; set; }

    public string Description { get; set; } = null!;

    public decimal ServiceCost { get; set; }

    public string VehicleLicencePlate { get; set; } = null!;

    public virtual Vehicle VehicleLicencePlateNavigation { get; set; } = null!;
}
