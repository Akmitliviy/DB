using System;
using System.Collections.Generic;

namespace ScaffoldDB.Migrations;

public partial class Office
{
    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    public virtual ICollection<Worker> Workers { get; set; } = new List<Worker>();
}
