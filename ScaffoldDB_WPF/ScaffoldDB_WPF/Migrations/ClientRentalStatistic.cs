using System;
using System.Collections.Generic;

namespace ScaffoldDB_WPF.Migrations;

public partial class ClientRentalStatistic
{
    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public long? TotalRents { get; set; }

    public decimal? AverageRentalDuration { get; set; }
}
