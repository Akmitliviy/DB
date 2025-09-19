using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Lab08DB.Models.CarRents;

namespace Lab08DB.Data
{
    public partial class CarRentsContext : DbContext
    {
        public CarRentsContext()
        {
        }

        public CarRentsContext(DbContextOptions<CarRentsContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Lab08DB.Models.CarRents.DamageReport>()
              .HasOne(i => i.Rent)
              .WithMany(i => i.DamageReports)
              .HasForeignKey(i => i.RentId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<Lab08DB.Models.CarRents.InsurancePolicy>()
              .HasOne(i => i.Vehicle)
              .WithMany(i => i.InsurancePolicies)
              .HasForeignKey(i => i.VehicleLicensePlate)
              .HasPrincipalKey(i => i.LicensePlate);

            builder.Entity<Lab08DB.Models.CarRents.Invoice>()
              .HasOne(i => i.Rent)
              .WithMany(i => i.Invoices)
              .HasForeignKey(i => i.RentId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<Lab08DB.Models.CarRents.Rent>()
              .HasOne(i => i.Client)
              .WithMany(i => i.Rents)
              .HasForeignKey(i => i.ClientEmail)
              .HasPrincipalKey(i => i.Email);

            builder.Entity<Lab08DB.Models.CarRents.Rent>()
              .HasOne(i => i.Vehicle)
              .WithMany(i => i.Rents)
              .HasForeignKey(i => i.VehicleLicensePlate)
              .HasPrincipalKey(i => i.LicensePlate);

            builder.Entity<Lab08DB.Models.CarRents.Rent>()
              .HasOne(i => i.Worker)
              .WithMany(i => i.Rents)
              .HasForeignKey(i => i.WorkerId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<Lab08DB.Models.CarRents.Review>()
              .HasOne(i => i.Client)
              .WithMany(i => i.Reviews)
              .HasForeignKey(i => i.ClientEmail)
              .HasPrincipalKey(i => i.Email);

            builder.Entity<Lab08DB.Models.CarRents.Review>()
              .HasOne(i => i.Vehicle)
              .WithMany(i => i.Reviews)
              .HasForeignKey(i => i.VehicleLicensePlate)
              .HasPrincipalKey(i => i.LicensePlate);

            builder.Entity<Lab08DB.Models.CarRents.ServiceRecord>()
              .HasOne(i => i.Vehicle)
              .WithMany(i => i.ServiceRecords)
              .HasForeignKey(i => i.VehicleLicencePlate)
              .HasPrincipalKey(i => i.LicensePlate);

            builder.Entity<Lab08DB.Models.CarRents.Vehicle>()
              .HasOne(i => i.Office)
              .WithMany(i => i.Vehicles)
              .HasForeignKey(i => i.OfficeName)
              .HasPrincipalKey(i => i.Name);

            builder.Entity<Lab08DB.Models.CarRents.Worker>()
              .HasOne(i => i.Office)
              .WithMany(i => i.Workers)
              .HasForeignKey(i => i.OfficeName)
              .HasPrincipalKey(i => i.Name);

            builder.Entity<Lab08DB.Models.CarRents.Rent>()
              .Property(p => p.Status)
              .HasDefaultValueSql(@"'Proccessed'::character varying");

            builder.Entity<Lab08DB.Models.CarRents.DamageReport>()
              .Property(p => p.RepairCost)
              .HasPrecision(18,2);

            builder.Entity<Lab08DB.Models.CarRents.InsurancePolicy>()
              .Property(p => p.Cost)
              .HasPrecision(18,2);

            builder.Entity<Lab08DB.Models.CarRents.Invoice>()
              .Property(p => p.TotalCost)
              .HasPrecision(18,2);

            builder.Entity<Lab08DB.Models.CarRents.Rent>()
              .Property(p => p.Cost)
              .HasPrecision(18,2);

            builder.Entity<Lab08DB.Models.CarRents.ServiceRecord>()
              .Property(p => p.ServiceCost)
              .HasPrecision(18,2);

            builder.Entity<Lab08DB.Models.CarRents.Vehicle>()
              .Property(p => p.CostPerDay)
              .HasPrecision(18,2);
            this.OnModelBuilding(builder);
        }

        public DbSet<Lab08DB.Models.CarRents.Client> Clients { get; set; }

        public DbSet<Lab08DB.Models.CarRents.DamageReport> DamageReports { get; set; }

        public DbSet<Lab08DB.Models.CarRents.InsurancePolicy> InsurancePolicies { get; set; }

        public DbSet<Lab08DB.Models.CarRents.Invoice> Invoices { get; set; }

        public DbSet<Lab08DB.Models.CarRents.Office> Offices { get; set; }

        public DbSet<Lab08DB.Models.CarRents.Rent> Rents { get; set; }

        public DbSet<Lab08DB.Models.CarRents.Review> Reviews { get; set; }

        public DbSet<Lab08DB.Models.CarRents.ServiceRecord> ServiceRecords { get; set; }

        public DbSet<Lab08DB.Models.CarRents.GetClientRentHistoryResult> GetClientRentHistoryResults { get; set; }

        public DbSet<Lab08DB.Models.CarRents.Vehicle> Vehicles { get; set; }

        public DbSet<Lab08DB.Models.CarRents.Worker> Workers { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }
}