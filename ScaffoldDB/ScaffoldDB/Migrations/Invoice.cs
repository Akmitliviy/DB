using System;
using System.Collections.Generic;

namespace ScaffoldDB.Migrations;

public partial class Invoice
{
    public Guid Id { get; set; }

    public DateOnly PayTerm { get; set; }

    public decimal TotalCost { get; set; }

    public string PaymentType { get; set; } = null!;

    public Guid RentId { get; set; }

    public virtual Rent Rent { get; set; } = null!;
}
