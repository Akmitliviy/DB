using System;
using System.Collections.Generic;

namespace ScaffoldDB.Migrations;

public partial class Rating
{
    public Guid Id { get; set; }

    public int Rating1 { get; set; }

    public string? Comment { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
