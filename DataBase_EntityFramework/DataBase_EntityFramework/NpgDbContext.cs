using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace DataBase_EntityFramework;

public sealed class NpgDbContext : DbContext
{
    
    public DbSet<Worker> Workers { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Rent> Rents { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Office> Offices { get; set; }
    public DbSet<InsurancePolicy> InsurancePolicies { get; set; }
    public DbSet<DamageReport> DamageReports { get; set; }
    public DbSet<ServiceRecord> ServiceRecords { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Worker has an optional relationship with Rent (one-to-one)
        modelBuilder.Entity<Worker>()
            .HasOne(w => w.Rent)
            .WithOne(r => r.Worker)
            .HasForeignKey<Worker>(w => w.RentId)
            .IsRequired(false); // Nullable foreign key

        // Worker has a required relationship with Office (many-to-one)
        modelBuilder.Entity<Worker>()
            .HasOne(w => w.Office)
            .WithMany(o => o.Workers)
            .HasForeignKey(w => w.OfficeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Rent has a required relationship with Vehicle (one-to-one)
        modelBuilder.Entity<Rent>()
            .HasOne(r => r.Vehicle)
            .WithOne(v => v.Rent)
            .HasForeignKey<Rent>(r => r.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Rent has a required relationship with Client (many-to-one)
        modelBuilder.Entity<Rent>()
            .HasOne(r => r.Client)
            .WithMany(c => c.Rents)
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Payment has a required relationship with Rent (many-to-one)
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Rent)
            .WithMany(r => r.Payments)
            .HasForeignKey(p => p.RentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Vehicle has a required relationship with Office (many-to-one)
        modelBuilder.Entity<Vehicle>()
            .HasOne(a => a.Office)
            .WithMany(o => o.Vehicles)
            .HasForeignKey(a => a.OfficeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Fluent API for InsurancePolicy (optional if relations are straightforward)
        modelBuilder.Entity<InsurancePolicy>()
            .HasOne(p => p.Rent)
            .WithMany(r => r.InsurancePolicies)
            .HasForeignKey(p => p.RentId)
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
            .HasForeignKey(s => s.AutoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Fluent API for Review
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Client)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Rent)
            .WithMany(re => re.Reviews)
            .HasForeignKey(r => r.RentId)
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