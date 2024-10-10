using Npgsql;

namespace DataBase_EntityFramework;

public class Program
{
    public static void Main(string[] args)
    {
        var context = new NpgDbContext();
        
        Populate(context);
    }

    private static void Populate(NpgDbContext context)
    {
        
            if (!context.Offices.Any())
            {
                // Add sample office data
                var office = new Office
                {
                    Address = "123 Main St",
                    Name = "Main Office",
                    PhoneNumber = "123456789"
                };

                context.Offices.Add(office);
                context.SaveChanges();

                // Add workers, autos, and other entities tied to the office
                var worker = new Worker
                {
                    FirstName = "John",
                    LastName = "Doe",
                    PhoneNumber = "987654321",
                    OccupationalPosition = "Manager",
                    OfficeId = office.ID
                };

                var vehicle = new Vehicle
                {
                    LicensePlate = "ABC123",
                    Model = "Toyota Camry",
                    Year = 2020,
                    FuelType = "Gasoline",
                    Mileage = 50000,
                    CostPerDay = 50.0m,
                    OfficeId = office.ID
                };

                context.Workers.Add(worker);
                context.Vehicles.Add(vehicle);
                context.SaveChanges();

                // Add a sample client and rent transaction
                var client = new Client
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    PhoneNumber = "123123123",
                    Email = "jane@example.com",
                    DriverLicense = "D123456",
                    BirthDate = new DateTime(2000, 2,6)
                };

                var rent = new Rent
                {
                    ClientId = client.ID,
                    VehicleId = vehicle.ID,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(7),
                    Cost = 350.0m,
                    Status = "Pending"
                };

                context.Clients.Add(client);
                context.Rents.Add(rent);
                context.SaveChanges();

                // Optionally add payments related to this rent
                var payment = new Payment
                {
                    RentId = rent.ID,
                    PayDay2 = DateTime.Now,
                    TotalCost = 350.0m,
                    PaymentType = "Credit Card"
                };

                context.Payments.Add(payment);
                context.SaveChanges();
            }
    }
}