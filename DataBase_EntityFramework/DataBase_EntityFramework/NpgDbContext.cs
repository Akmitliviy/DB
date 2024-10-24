using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace DataBase_EntityFramework;

public sealed class NpgDbContext : DbContext
{
    
    public DbSet<Worker> Workers { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Rent> Rents { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Office> Offices { get; set; }
    public DbSet<InsurancePolicy> InsurancePolicies { get; set; }
    public DbSet<DamageReport> DamageReports { get; set; }
    public DbSet<ServiceRecord> ServiceRecords { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Worker has an optional relationship with Rent (one-to-one)
        modelBuilder.Entity<Rent>()
            .HasOne(r => r.Worker)
            .WithMany(w => w.Rents)
            .HasForeignKey(r => r.WorkerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Worker has a required relationship with Office (many-to-one)
        modelBuilder.Entity<Worker>()
            .HasOne(w => w.Office)
            .WithMany(o => o.Workers)
            .HasForeignKey(w => w.OfficeName)
            .OnDelete(DeleteBehavior.Cascade);

        // Rent has a required relationship with Vehicle (one-to-one)
        modelBuilder.Entity<Rent>()
            .HasOne(r => r.Vehicle)
            .WithMany(v => v.Rents)
            .HasForeignKey(r => r.VehicleLicensePlate)
            .OnDelete(DeleteBehavior.Restrict);

        // Rent has a required relationship with Client (many-to-one)
        modelBuilder.Entity<Rent>()
            .HasOne(r => r.Client)
            .WithMany(c => c.Rents)
            .HasForeignKey(r => r.ClientEmail)
            .OnDelete(DeleteBehavior.Cascade);

        // Payment has a required relationship with Rent (many-to-one)
        modelBuilder.Entity<Invoice>()
            .HasOne(p => p.Rent)
            .WithOne(r => r.Invoice)
            .HasForeignKey<Invoice>(p => p.RentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Vehicle has a required relationship with Office (many-to-one)
        modelBuilder.Entity<Vehicle>()
            .HasOne(a => a.Office)
            .WithMany(o => o.Vehicles)
            .HasForeignKey(a => a.OfficeName)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Fluent API for InsurancePolicy (optional if relations are straightforward)
        modelBuilder.Entity<InsurancePolicy>()
            .HasOne(p => p.Vehicle)
            .WithMany(r => r.InsurancePolicies)
            .HasForeignKey(p => p.VehicleLicensePlate)
            .OnDelete(DeleteBehavior.Restrict);

        // Fluent API for DamageReport
        modelBuilder.Entity<DamageReport>()
            .HasOne(d => d.Rent)
            .WithMany(r => r.DamageReports)
            .HasForeignKey(d => d.RentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Fluent API for ServiceRecord
        modelBuilder.Entity<ServiceRecord>()
            .HasOne(s => s.Vehicle)
            .WithMany(a => a.ServiceRecords)
            .HasForeignKey(s => s.VehicleLicencePlate)
            .OnDelete(DeleteBehavior.Restrict);

        // Fluent API for Review
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Client)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.ClientEmail)
            .OnDelete(DeleteBehavior.Restrict);

        // Fluent API for Review
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Vehicle)
            .WithMany(v => v.Reviews)
            .HasForeignKey(r => r.VehicleLicensePlate)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public NpgDbContext()
    {
        Database.EnsureCreated();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=CarRent;Username=postgres;Password=baest4rd");
    }
}