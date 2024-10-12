﻿namespace DataBase_EntityFramework;

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
            OfficeName = "HeadQuarters"
        },
        new()
        {
            LicensePlate = "AE 7345 AB",
            Model = "Porsche Carrera gt",
            Year = 2023,
            FuelType = "Gasoline",
            Mileage = 13000,
            CostPerDay = 16000,
            OfficeName = "New York"
        }
    ];
    
    private static List<Worker> Workers =
    [
        new()
        {
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "555-123-4567",
            OccupationalPosition = "Manager",
            OfficeName = "HeadQuarters" // Linked to HeadQuarters office
        },

        new()
        {
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
            VehicleLicensePlate = "BC 4534 BB", // Honda NSX
            ClientEmail = "alice.johnson@email.com", // Alice Johnson
            StartDate = new DateOnly(2024, 10, 1),
            EndDate = new DateOnly(2024, 10, 5),
            Cost = 70000,
            Status = "Completed",
            WorkerId = 1
        },

        new()
        {
            VehicleLicensePlate = "AE 7345 AB", // Porsche Carrera GT
            ClientEmail = "bob.brown@email.com", // Bob Brown
            StartDate = new DateOnly(2024, 10, 10),
            EndDate = new DateOnly(2024, 10, 15),
            Cost = 80000,
            Status = "Ongoing",
            WorkerId = 2
        }
    ];
    
    private static List<Invoice> Invoices =
    [
        new()
        {
            RentId = 1, // Rent for Honda NSX
            PayTerm = new DateOnly(2024, 10, 1),
            TotalCost = 70000,
            PaymentType = "Credit Card"
        },

        new()
        {
            RentId = 2, // Rent for Porsche Carrera GT
            PayTerm = new DateOnly(2024, 10, 10),
            TotalCost = 80000,
            PaymentType = "Cash"
        }
    ];
    private static List<InsurancePolicy> InsurancePolicies =
    [
        new()
        {
            PolicyNumber = "INS-123456",
            Provider = "ABC Insurance",
            Cost = 5000,
            StartDate = new DateOnly(2024, 9, 30),
            EndDate = new DateOnly(2024, 10, 5),
            VehicleLicensePlate = "BC 4534 BB" // Honda NSX Rent
        },

        new()
        {
            PolicyNumber = "INS-654321",
            Provider = "XYZ Insurance",
            Cost = 6000,
            StartDate = new DateOnly(2024, 10, 9),
            EndDate = new DateOnly(2024, 10, 15),
            VehicleLicensePlate = "AE 7345 AB" // Porsche Carrera GT Rent
        }
    ];
    private static List<DamageReport> DamageReports =
    [
        new()
        {
            Description = "Minor scratch on front bumper",
            RepairCost = 1000,
            ReportDate = new DateOnly(2024, 10, 5),
            RentId = 1 // Honda NSX Rent
        },

        new()
        {
            Description = "Broken tail light",
            RepairCost = 2000,
            ReportDate = new DateOnly(2024, 10, 12),
            RentId = 2 // Porsche Carrera GT Rent
        }
    ];
    private static List<ServiceRecord> ServiceRecords =
    [
        new()
        {
            ServiceDate = new DateOnly(2024, 9, 20),
            Description = "Oil change and tire rotation",
            ServiceCost = 3000,
            VehicleLicencePlate = "BC 4534 BB" // Honda NSX
        },

        new()
        {
            ServiceDate = new DateOnly(2024, 9, 25),
            Description = "Brake pad replacement",
            ServiceCost = 4000,
            VehicleLicencePlate = "AE 7345 AB" // Porsche Carrera GT
        }
    ];
    private static List<Review> Reviews =
    [
        new()
        {
            Rating = 5,
            Comment = "Great car and service!",
            ReviewDate = new DateOnly(2024, 10, 6),
            ClientEmail = "alice.johnson@email.com", // Alice Johnson
            RentId = 1 // Honda NSX Rent
        },

        new()
        {
            Rating = 4,
            Comment = "Good experience, but the car had some issues.",
            ReviewDate = new DateOnly(2024, 10, 16),
            ClientEmail = "bob.brown@email.com", // Bob Brown
            RentId = 2 // Porsche Carrera GT Rent
        }
    ];




    public static void Main(string[] args)
    {
        var context = new NpgDbContext();

        Populate(context);
    }

    private static void Populate(NpgDbContext context)
    {
        // foreach (var entity in Offices)
        // {
        //     context.Offices.Add(entity);
        //     context.SaveChanges();
        // }
        // foreach (var entity in Vehicles)
        // {
        //     context.Vehicles.Add(entity);
        //     context.SaveChanges();
        // }
        // foreach (var entity in Workers)
        // {
        //     context.Workers.Add(entity);
        //     context.SaveChanges();
        // }
        // foreach (var entity in Clients)
        // {
        //     context.Clients.Add(entity);
        //     context.SaveChanges();
        // }
        // foreach (var entity in Rents)
        // {
        //     context.Rents.Add(entity);
        //     context.SaveChanges();
        // }
        // foreach (var entity in Invoices)
        // {
        //     context.Invoices.Add(entity);
        //     context.SaveChanges();
        // }
        // foreach (var entity in InsurancePolicies)
        // {
        //     context.InsurancePolicies.Add(entity);
        //     context.SaveChanges();
        // }
        // foreach (var entity in DamageReports)
        // {
        //     context.DamageReports.Add(entity);
        //     context.SaveChanges();
        // }
        // foreach (var entity in ServiceRecords)
        // {
        //     context.ServiceRecords.Add(entity);
        //     context.SaveChanges();
        // }
        // foreach (var entity in Reviews)
        // {
        //     context.Reviews.Add(entity);
        //     context.SaveChanges();
        // }
    }
}