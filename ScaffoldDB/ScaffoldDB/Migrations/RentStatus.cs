using System;
using System.Collections.Generic;

namespace ScaffoldDB.Migrations;

public partial class RentStatus
{
    public string Status { get; set; } = null!;

    public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();
}
