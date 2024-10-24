using System;
using System.Collections.Generic;

namespace ScaffoldDB.Migrations;

public partial class Vehicle
{
    public string LicensePlate { get; set; } = null!;

    public string Model { get; set; } = null!;

    public int Year { get; set; }

    public string FuelType { get; set; } = null!;

    public int Mileage { get; set; }

    public decimal CostPerDay { get; set; }

    public string OfficeName { get; set; } = null!;

    public virtual ICollection<InsurancePolicy> InsurancePolicies { get; set; } = new List<InsurancePolicy>();

    public virtual Office OfficeNameNavigation { get; set; } = null!;

    public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<ServiceRecord> ServiceRecords { get; set; } = new List<ServiceRecord>();
}
