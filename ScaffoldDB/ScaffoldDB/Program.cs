using ScaffoldDB.Context;
using ScaffoldDB.Migrations;

namespace ScaffoldDB;

public class Program
{

    public static void Main(string[] args)
    {
        var context = new MyDbContext();

        Populate(context);
    }

    private static void Populate(MyDbContext context)
    {
        /*
         Offices
         Vehicles
         Workers
         Clients
         Rents
         Invoices
         InsurancePolicies
         DamageReports
         ServiceRecords
         Reviews
         */
        
        var generator = new DataGenerator();
        
        var offices = generator.GenerateOffices(15);
        context.AddRange(offices);
        context.SaveChanges();
        
        var vehicles = generator.GenerateVehicles(100);
        context.AddRange(vehicles);
        context.SaveChanges();
        
        var workers = generator.GenerateWorkers(200);
        context.AddRange(workers);
        context.SaveChanges();
        
        var clients = generator.GenerateClients(150);
        context.AddRange(clients);
        context.SaveChanges();
        
        var rents = generator.GenerateRents(clients.Count);
        context.AddRange(rents);
        context.SaveChanges();
        
        var invoices = generator.GenerateInvoices(clients.Count);
        context.AddRange(invoices);
        context.SaveChanges();
        
        var insurancePolicies = generator.GenerateInsurancePolicies(vehicles.Count * 2);
        context.AddRange(insurancePolicies);
        context.SaveChanges();
        
        var damageReports = generator.GenerateDamageReports(vehicles.Count / 10);
        context.AddRange(damageReports);
        context.SaveChanges();
        
        var serviceRecords = generator.GenerateServiceRecords(vehicles.Count * 2);
        context.AddRange(serviceRecords);
        context.SaveChanges();
        
        var reviews = generator.GenerateReviews(clients.Count / 3);
        context.AddRange(reviews);
        context.SaveChanges();
        
    }
}