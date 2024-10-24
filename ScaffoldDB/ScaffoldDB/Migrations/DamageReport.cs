using System;
using System.Collections.Generic;

namespace ScaffoldDB.Migrations;

public partial class DamageReport
{
    public Guid Id { get; set; }

    public string Description { get; set; } = null!;

    public decimal RepairCost { get; set; }

    public DateOnly ReportDate { get; set; }

    public Guid RentId { get; set; }

    public virtual Rent Rent { get; set; } = null!;
}
