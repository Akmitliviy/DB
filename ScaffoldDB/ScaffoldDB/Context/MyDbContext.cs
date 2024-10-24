using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ScaffoldDB.Migrations;

namespace ScaffoldDB.Context;

public sealed partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
        Database.EnsureCreated();
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Client> Clients { get; set; }

    public DbSet<DamageReport> DamageReports { get; set; }

    public DbSet<InsurancePolicy> InsurancePolicies { get; set; }

    public DbSet<Invoice> Invoices { get; set; }

    public DbSet<Office> Offices { get; set; }

    public DbSet<Rent> Rents { get; set; }

    public DbSet<Review> Reviews { get; set; }

    public DbSet<ServiceRecord> ServiceRecords { get; set; }

    public DbSet<Vehicle> Vehicles { get; set; }

    public DbSet<Worker> Workers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=TestDB;Username=postgres;Password=baest4rd");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Email);

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.DriverLicense).HasMaxLength(20);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
        });

        modelBuilder.Entity<DamageReport>(entity =>
        {
            entity.HasIndex(e => e.RentId, "IX_DamageReports_RentId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.RepairCost).HasPrecision(18, 2);

            entity.HasOne(d => d.Rent).WithMany(p => p.DamageReports)
                .HasForeignKey(d => d.RentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InsurancePolicy>(entity =>
        {
            entity.HasIndex(e => e.VehicleLicensePlate, "IX_InsurancePolicies_VehicleLicensePlate");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Cost).HasPrecision(18, 2);
            entity.Property(e => e.PolicyNumber).HasMaxLength(20);
            entity.Property(e => e.Provider).HasMaxLength(50);
            entity.Property(e => e.VehicleLicensePlate).HasMaxLength(20);

            entity.HasOne(d => d.VehicleLicensePlateNavigation).WithMany(p => p.InsurancePolicies)
                .HasForeignKey(d => d.VehicleLicensePlate)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasIndex(e => e.RentId, "IX_Invoices_RentId").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.PaymentType).HasMaxLength(30);
            entity.Property(e => e.TotalCost).HasPrecision(18, 2);

            entity.HasOne(d => d.Rent).WithOne(p => p.Invoice)
                .HasForeignKey<Invoice>(d => d.RentId)
                .HasConstraintName("fk_invoices_rents_rentid");
        });

        modelBuilder.Entity<Office>(entity =>
        {
            entity.HasKey(e => e.Name);

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
        });

        modelBuilder.Entity<Rent>(entity =>
        {
            entity.HasIndex(e => e.ClientEmail, "IX_Rents_ClientEmail");

            entity.HasIndex(e => e.VehicleLicensePlate, "IX_Rents_VehicleLicensePlate");

            entity.HasIndex(e => e.WorkerId, "IX_Rents_WorkerId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ClientEmail).HasMaxLength(100);
            entity.Property(e => e.Cost).HasPrecision(18, 2);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Proccessed'::character varying");
            entity.Property(e => e.VehicleLicensePlate).HasMaxLength(20);

            entity.HasOne(d => d.ClientEmailNavigation).WithMany(p => p.Rents)
                .HasForeignKey(d => d.ClientEmail)
                .HasConstraintName("fk_rents_clients_clientemail");

            entity.HasOne(d => d.VehicleLicensePlateNavigation).WithMany(p => p.Rents)
                .HasForeignKey(d => d.VehicleLicensePlate)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Worker).WithMany(p => p.Rents)
                .HasForeignKey(d => d.WorkerId)
                .HasConstraintName("fk_rents_workers_workerid");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasIndex(e => e.ClientEmail, "IX_Reviews_ClientEmail");

            entity.HasIndex(e => e.VehicleLicensePlate, "IX_Reviews_VehicleLicensePlate");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ClientEmail).HasMaxLength(100);
            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.VehicleLicensePlate).HasMaxLength(20);

            entity.HasOne(d => d.ClientEmailNavigation).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ClientEmail)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.VehicleLicensePlateNavigation).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.VehicleLicensePlate)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ServiceRecord>(entity =>
        {
            entity.HasIndex(e => e.VehicleLicencePlate, "IX_ServiceRecords_VehicleLicencePlate");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ServiceCost).HasPrecision(18, 2);
            entity.Property(e => e.VehicleLicencePlate).HasMaxLength(20);

            entity.HasOne(d => d.VehicleLicencePlateNavigation).WithMany(p => p.ServiceRecords)
                .HasForeignKey(d => d.VehicleLicencePlate)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.LicensePlate);

            entity.HasIndex(e => e.OfficeName, "IX_Vehicles_OfficeName");

            entity.Property(e => e.LicensePlate).HasMaxLength(20);
            entity.Property(e => e.CostPerDay).HasPrecision(18, 2);
            entity.Property(e => e.FuelType).HasMaxLength(20);
            entity.Property(e => e.Model).HasMaxLength(50);
            entity.Property(e => e.OfficeName).HasMaxLength(50);

            entity.HasOne(d => d.OfficeNameNavigation).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.OfficeName)
                .HasConstraintName("fk_vehicles_offices_officename");
        });

        modelBuilder.Entity<Worker>(entity =>
        {
            entity.HasIndex(e => e.OfficeName, "IX_Workers_OfficeName");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.OccupationalPosition).HasMaxLength(50);
            entity.Property(e => e.OfficeName).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);

            entity.HasOne(d => d.OfficeNameNavigation).WithMany(p => p.Workers)
                .HasForeignKey(d => d.OfficeName)
                .HasConstraintName("fk_workers_offices_officename");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
