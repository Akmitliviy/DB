using ScaffoldDB.Context;
using ScaffoldDB.Migrations;

namespace ScaffoldDB;

public class Program
{
    private static List<Office> Offices =
    [
        new()
        {
            Address = "London, Linq st., 45",
            Name = "HeadQuarters",
            PhoneNumber = "555-555-5555"
        }, 
        new ()
        {
            Address = "Washington, Pattern st., 77",
            Name = "Washington",
            PhoneNumber = "666-666-6666"
        },
        new()
        {
            Address = "New York, Regex st., 13",
            Name = "New York",
            PhoneNumber = "222-222-2222"
        }
    ];

    private static List<Vehicle> Vehicles =
    [
        new()
        {
            LicensePlate = "BC 4534 BB",
            Model = "Honda NSX",
            Year = 2020,
            FuelType = "Gasoline",
            Mileage = 25000,
            CostPerDay = 14000,
            OfficeName = Offices[0].Name
        },
        new()
        {
            LicensePlate = "AE 7345 AB",
            Model = "Porsche Carrera gt",
            Year = 2023,
            FuelType = "Gasoline",
            Mileage = 13000,
            CostPerDay = 16000,
            OfficeName = Offices[2].Name
        }
    ];
    
    private static List<Worker> Workers =
    [
        new()
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "555-123-4567",
            OccupationalPosition = "Manager",
            OfficeName = "HeadQuarters" // Linked to HeadQuarters office
        },

        new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Smith",
            PhoneNumber = "666-987-6543",
            OccupationalPosition = "Receptionist",
            OfficeName = "Washington" // Linked to Washington office
        }
    ];
    private static List<Client> Clients =
    [
        new()
        {
            FirstName = "Alice",
            LastName = "Johnson",
            PhoneNumber = "555-765-4321",
            Email = "alice.johnson@email.com",
            DriverLicense = "DL12345",
            BirthDate = new DateOnly(1987, 5, 15)
        },

        new()
        {
            FirstName = "Bob",
            LastName = "Brown",
            PhoneNumber = "666-543-2109",
            Email = "bob.brown@email.com",
            DriverLicense = "DL67890",
            BirthDate = new DateOnly(1990, 8, 21)
        }
    ];
    private static List<Rent> Rents =
    [
        new()
        {
            Id = Guid.NewGuid(),
            VehicleLicensePlate = "BC 4534 BB", // Honda NSX
            ClientEmail = "alice.johnson@email.com", // Alice Johnson
            StartDate = new DateOnly(2024, 10, 1),
            EndDate = new DateOnly(2024, 10, 5),
            Cost = 70000,
            Status = "Completed",
            Worker = Workers[0]
        },

        new()
        {
            Id = Guid.NewGuid(),
            VehicleLicensePlate = "AE 7345 AB", // Porsche Carrera GT
            ClientEmail = "bob.brown@email.com", // Bob Brown
            StartDate = new DateOnly(2024, 10, 10),
            EndDate = new DateOnly(2024, 10, 15),
            Cost = 80000,
            Status = "Ongoing",
            Worker = Workers[1]
        }
    ];
    
    private static List<Invoice> Invoices =
    [
        new()
        {
            Id = Guid.NewGuid(),
            Rent = Rents[0], // Rent for Honda NSX
            PayTerm = new DateOnly(2024, 10, 1),
            TotalCost = 70000,
            PaymentType = "Credit Card"
        },

        new()
        {
            Id = Guid.NewGuid(),
            Rent = Rents[1], // Rent for Porsche Carrera GT
            PayTerm = new DateOnly(2024, 10, 10),
            TotalCost = 80000,
            PaymentType = "Cash"
        }
    ];
    private static List<InsurancePolicy> InsurancePolicies =
    [
        new()
        {
            Id = Guid.NewGuid(),
            PolicyNumber = "INS-123456",
            Provider = "ABC Insurance",
            Cost = 5000,
            StartDate = new DateOnly(2024, 9, 30),
            EndDate = new DateOnly(2024, 10, 5),
            VehicleLicensePlate = Vehicles[0].LicensePlate // Honda NSX Rent
        },

        new()
        {
            Id = Guid.NewGuid(),
            PolicyNumber = "INS-654321",
            Provider = "XYZ Insurance",
            Cost = 6000,
            StartDate = new DateOnly(2024, 10, 9),
            EndDate = new DateOnly(2024, 10, 15),
            VehicleLicensePlate = Vehicles[1].LicensePlate // Porsche Carrera GT Rent
        }
    ];
    private static List<DamageReport> DamageReports =
    [
        new()
        {
            Id = Guid.NewGuid(),
            Description = "Minor scratch on front bumper",
            RepairCost = 1000,
            ReportDate = new DateOnly(2024, 10, 5),
            Rent = Rents[0] // Honda NSX Rent
        },

        new()
        {
            Id = Guid.NewGuid(),
            Description = "Broken tail light",
            RepairCost = 2000,
            ReportDate = new DateOnly(2024, 10, 12),
            Rent = Rents[1] // Porsche Carrera GT Rent
        }
    ];
    private static List<ServiceRecord> ServiceRecords =
    [
        new()
        {
            Id = Guid.NewGuid(),
            ServiceDate = new DateOnly(2024, 9, 20),
            Description = "Oil change and tire rotation",
            ServiceCost = 3000,
            VehicleLicencePlate = Vehicles[0].LicensePlate // Honda NSX
        },

        new()
        {
            Id = Guid.NewGuid(),
            ServiceDate = new DateOnly(2024, 9, 25),
            Description = "Brake pad replacement",
            ServiceCost = 4000,
            VehicleLicencePlate = Vehicles[1].LicensePlate // Porsche Carrera GT
        }
    ];
    private static List<Review> Reviews =
    [
        new()
        {
            Id = Guid.NewGuid(),
            Rating = 5,
            Comment = "Great car and service!",
            ReviewDate = new DateOnly(2024, 10, 6),
            ClientEmail = "alice.johnson@email.com", // Alice Johnson
            VehicleLicensePlate = Vehicles[0].LicensePlate// Honda NSX Rent
        },

        new()
        {
            Id = Guid.NewGuid(),
            Rating = 4,
            Comment = "Good experience, but the car had some issues.",
            ReviewDate = new DateOnly(2024, 10, 16),
            ClientEmail = "bob.brown@email.com", // Bob Brown
            VehicleLicensePlate = Vehicles[1].LicensePlate // Porsche Carrera GT Rent
        }
    ];




    public static void Main(string[] args)
    {
        var context = new MyDbContext();

        Populate(context);
    }

    private static void Populate(MyDbContext context)
    {
        foreach (var entity in Offices)
        {
            context.Offices.Add(entity);
            context.SaveChanges();
        }

        foreach (var entity in Vehicles)
        {
            context.Vehicles.Add(entity);
            context.SaveChanges();
        }

        foreach (var entity in Workers)
        {
            context.Workers.Add(entity);
            context.SaveChanges();
        }

        foreach (var entity in Clients)
        {
            context.Clients.Add(entity);
            context.SaveChanges();
        }

        foreach (var entity in Rents)
        {
            context.Rents.Add(entity);
            context.SaveChanges();
        }

        foreach (var entity in Invoices)
        {
            context.Invoices.Add(entity);
            context.SaveChanges();
        }

        foreach (var entity in InsurancePolicies)
        {
            context.InsurancePolicies.Add(entity);
            context.SaveChanges();
        }

        foreach (var entity in DamageReports)
        {
            context.DamageReports.Add(entity);
            context.SaveChanges();
        }

        foreach (var entity in ServiceRecords)
        {
            context.ServiceRecords.Add(entity);
            context.SaveChanges();
        }

        foreach (var entity in Reviews)
        {
            context.Reviews.Add(entity);
            context.SaveChanges();
        }
    }
}