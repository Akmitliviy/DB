using System;
using System.Collections.Generic;

namespace ScaffoldDB.Migrations;

public partial class Client
{
    public string Email { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string DriverLicense { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
