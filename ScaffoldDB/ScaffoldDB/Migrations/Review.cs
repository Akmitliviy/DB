using System;
using System.Collections.Generic;

namespace ScaffoldDB.Migrations;

public partial class Review
{
    public Guid Id { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateOnly ReviewDate { get; set; }

    public string ClientEmail { get; set; } = null!;

    public string VehicleLicensePlate { get; set; } = null!;

    public virtual Client ClientEmailNavigation { get; set; } = null!;

    public virtual Vehicle VehicleLicensePlateNavigation { get; set; } = null!;
}
