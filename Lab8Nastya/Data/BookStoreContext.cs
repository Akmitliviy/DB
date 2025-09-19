using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Lab8Nastya.Models.BookStore;

namespace Lab8Nastya.Data
{
    public partial class BookStoreContext : DbContext
    {
        public BookStoreContext()
        {
        }

        public BookStoreContext(DbContextOptions<BookStoreContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Lab8Nastya.Models.BookStore.AddNewOrderReceipt>().HasNoKey();

            builder.Entity<Lab8Nastya.Models.BookStore.GetClientsByOrderStatus>().HasNoKey();

            builder.Entity<Lab8Nastya.Models.BookStore.SpAlterdiagram>().HasNoKey();

            builder.Entity<Lab8Nastya.Models.BookStore.SpCreatediagram>().HasNoKey();

            builder.Entity<Lab8Nastya.Models.BookStore.SpDropdiagram>().HasNoKey();

            builder.Entity<Lab8Nastya.Models.BookStore.SpHelpdiagramdefinition>().HasNoKey();

            builder.Entity<Lab8Nastya.Models.BookStore.SpHelpdiagram>().HasNoKey();

            builder.Entity<Lab8Nastya.Models.BookStore.SpRenamediagram>().HasNoKey();

            builder.Entity<Lab8Nastya.Models.BookStore.SpUpgraddiagram>().HasNoKey();

            builder.Entity<Lab8Nastya.Models.BookStore.UpdateOrderTotal>().HasNoKey();

            builder.Entity<Lab8Nastya.Models.BookStore.BookAmountLocation>()
              .HasOne(i => i.Book)
              .WithMany(i => i.BookAmountLocations)
              .HasForeignKey(i => i.BookId)
              .HasPrincipalKey(i => i.BookId);

            builder.Entity<Lab8Nastya.Models.BookStore.BookAmountLocation>()
              .HasOne(i => i.Location)
              .WithMany(i => i.BookAmountLocations)
              .HasForeignKey(i => i.LocationId)
              .HasPrincipalKey(i => i.LocationId);

            builder.Entity<Lab8Nastya.Models.BookStore.BookAmountOrder>()
              .HasOne(i => i.Book)
              .WithMany(i => i.BookAmountOrders)
              .HasForeignKey(i => i.BookId)
              .HasPrincipalKey(i => i.BookId);

            builder.Entity<Lab8Nastya.Models.BookStore.BookAmountOrder>()
              .HasOne(i => i.Order)
              .WithMany(i => i.BookAmountOrders)
              .HasForeignKey(i => i.OrderId)
              .HasPrincipalKey(i => i.OrderId);

            builder.Entity<Lab8Nastya.Models.BookStore.Book>()
              .HasOne(i => i.Author)
              .WithMany(i => i.Books)
              .HasForeignKey(i => i.AuthorId)
              .HasPrincipalKey(i => i.AuthorId);

            builder.Entity<Lab8Nastya.Models.BookStore.Book>()
              .HasOne(i => i.Category)
              .WithMany(i => i.Books)
              .HasForeignKey(i => i.CategoryId)
              .HasPrincipalKey(i => i.CategoryId);

            builder.Entity<Lab8Nastya.Models.BookStore.Book>()
              .HasOne(i => i.Publisher)
              .WithMany(i => i.Books)
              .HasForeignKey(i => i.PublisherId)
              .HasPrincipalKey(i => i.PublisherId);

            builder.Entity<Lab8Nastya.Models.BookStore.Order>()
              .HasOne(i => i.Client)
              .WithMany(i => i.Orders)
              .HasForeignKey(i => i.ClientId)
              .HasPrincipalKey(i => i.ClientId);

            builder.Entity<Lab8Nastya.Models.BookStore.Order>()
              .HasOne(i => i.Location)
              .WithMany(i => i.Orders)
              .HasForeignKey(i => i.LocationId)
              .HasPrincipalKey(i => i.LocationId);

            builder.Entity<Lab8Nastya.Models.BookStore.WorkingHour>()
              .HasOne(i => i.Location)
              .WithMany(i => i.WorkingHours)
              .HasForeignKey(i => i.LocationId)
              .HasPrincipalKey(i => i.LocationId);

            builder.Entity<Lab8Nastya.Models.BookStore.BookAmountLocation>()
              .Property(p => p.Quantity)
              .HasDefaultValueSql(@"((1))");

            builder.Entity<Lab8Nastya.Models.BookStore.BookAmountOrder>()
              .Property(p => p.Quantity)
              .HasDefaultValueSql(@"((1))");

            builder.Entity<Lab8Nastya.Models.BookStore.Book>()
              .Property(p => p.DiscountPercentage)
              .HasDefaultValueSql(@"((0))");

            builder.Entity<Lab8Nastya.Models.BookStore.Order>()
              .Property(p => p.OrderDate)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Lab8Nastya.Models.BookStore.Order>()
              .Property(p => p.Status)
              .HasDefaultValueSql(@"('paid')");

            builder.Entity<Lab8Nastya.Models.BookStore.Order>()
              .Property(p => p.ReceivingDate)
              .HasDefaultValueSql(@"(NULL)");

            builder.Entity<Lab8Nastya.Models.BookStore.Book>()
              .Property(p => p.FinalPrice)
              .HasComputedColumnSql(@"([price]-([price]*[discount_percentage])/(100))")
              .ValueGeneratedOnAddOrUpdate()
              .Metadata.SetBeforeSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);

            builder.Entity<Lab8Nastya.Models.BookStore.Client>()
              .Property(p => p.FullName)
              .HasComputedColumnSql(@"(([first_name]+' ')+[last_name])")
              .ValueGeneratedOnAddOrUpdate()
              .Metadata.SetBeforeSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);

            builder.Entity<Lab8Nastya.Models.BookStore.Order>()
              .Property(p => p.OrderDate)
              .HasColumnType("datetime");

            builder.Entity<Lab8Nastya.Models.BookStore.Order>()
              .Property(p => p.ReceivingDate)
              .HasColumnType("datetime");
            this.OnModelBuilding(builder);
        }

        public DbSet<Lab8Nastya.Models.BookStore.Author> Authors { get; set; }

        public DbSet<Lab8Nastya.Models.BookStore.BookAmountLocation> BookAmountLocations { get; set; }

        public DbSet<Lab8Nastya.Models.BookStore.BookAmountOrder> BookAmountOrders { get; set; }

        public DbSet<Lab8Nastya.Models.BookStore.Book> Books { get; set; }

        public DbSet<Lab8Nastya.Models.BookStore.Category> Categories { get; set; }

        public DbSet<Lab8Nastya.Models.BookStore.Client> Clients { get; set; }

        public DbSet<Lab8Nastya.Models.BookStore.Location> Locations { get; set; }

        public DbSet<Lab8Nastya.Models.BookStore.Order> Orders { get; set; }

        public DbSet<Lab8Nastya.Models.BookStore.Publisher> Publishers { get; set; }

        public DbSet<Lab8Nastya.Models.BookStore.WorkingHour> WorkingHours { get; set; }

        public DbSet<Lab8Nastya.Models.BookStore.AddNewOrderReceipt> AddNewOrderReceipts { get; set; }

        public DbSet<Lab8Nastya.Models.BookStore.GetClientsByOrderStatus> GetClientsByOrderStatuses { get; set; }

        public DbSet<Lab8Nastya.Models.BookStore.SpAlterdiagram> SpAlterdiagrams { get; set; }

        public DbSet<Lab8Nastya.Models.BookStore.SpCreatediagram> SpCreatediagrams { get; set; }

        public DbSet<Lab8Nastya.Models.BookStore.SpDropdiagram> SpDropdiagrams { get; set; }

        public DbSet<Lab8Nastya.Models.BookStore.SpHelpdiagramdefinition> SpHelpdiagramdefinitions { get; set; }

        public DbSet<Lab8Nastya.Models.BookStore.SpHelpdiagram> SpHelpdiagrams { get; set; }

        public DbSet<Lab8Nastya.Models.BookStore.SpRenamediagram> SpRenamediagrams { get; set; }

        public DbSet<Lab8Nastya.Models.BookStore.SpUpgraddiagram> SpUpgraddiagrams { get; set; }

        public DbSet<Lab8Nastya.Models.BookStore.UpdateOrderTotal> UpdateOrderTotals { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }
}