using Bogus;
using ScaffoldDB.Migrations;

namespace ScaffoldDB
{
    public class DataGenerator
    {
        private List<string> Emails { get; set; }
        private List<string> FirstNames { get; set; }
        private List<string> LastNames { get; set; }
        private List<string> PhoneNumbers { get; set; }
        private List<string> DriverLicenses { get; set; }
        private List<DateOnly> BirthDates { get; set; }
        private List<string> Descriptions { get; set; }
        private List<decimal> RepairCosts { get; set; }
        private List<DateOnly> ReportDates { get; set; }
        private List<string> PolicyNumbers { get; set; }
        private List<string> Providers { get; set; }
        private List<decimal> Costs { get; set; }
        private List<DateOnly> StartDates { get; set; }
        private List<DateOnly> EndDates { get; set; }
        private List<string> VehicleLicensePlates { get; set; }
        private List<decimal> TotalCosts { get; set; }
        private List<string> PaymentTypes { get; set; }
        private List<string> OfficeNames { get; set; }
        private List<string> Addresses { get; set; }
        private List<string> OccupationalPositions { get; set; }
        private List<string> FuelTypes { get; set; }
        private List<int> Mileages { get; set; }
        private List<string> CarModels{ get; set; }
        private List<DateOnly> CarYears { get; set; }
        private List<string> Statuses { get; set; }
        
        private List<Office> Offices { get; set; }
        private List<Client> Clients { get; set; }
        private List<Worker> Workers { get; set; }
        private List<Vehicle> Vehicles { get; set; }
        private List<Rent> Rents { get; set; }

        public DataGenerator()
        {
            var faker = new Faker();

            Emails = GenerateList(() => faker.Internet.Email());
            FirstNames = GenerateList(() => faker.Name.FirstName());
            LastNames = GenerateList(() => faker.Name.LastName());
            PhoneNumbers = GenerateList(() => faker.Phone.PhoneNumber("###-###-####"));
            DriverLicenses = GenerateList(() => faker.Random.String2(8, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"));
            BirthDates = GenerateList(() => DateOnly.FromDateTime(faker.Date.Past(50, DateTime.Now.AddYears(-18))));
            CarYears = GenerateList(() => DateOnly.FromDateTime(faker.Date.Past(10, DateTime.Now)));

            Descriptions = GenerateList(() => faker.Lorem.Sentence());
            RepairCosts = GenerateList(() => faker.Finance.Amount(50, 5000));
            ReportDates = GenerateList(() => DateOnly.FromDateTime(faker.Date.Past(2)));

            PolicyNumbers = GenerateList(() => faker.Random.String2(10, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"));
            Providers = GenerateList(() => faker.Company.CompanyName());
            Costs = GenerateList(() => faker.Finance.Amount(100, 2000));
            StartDates = GenerateList(() => DateOnly.FromDateTime(faker.Date.Past()));
            EndDates = GenerateList(() => DateOnly.FromDateTime(faker.Date.Future()));
            VehicleLicensePlates = GenerateList(() => faker.Random.String2(7, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"));

            TotalCosts = GenerateList(() => faker.Finance.Amount(100, 5000));
            PaymentTypes = GenerateList(() =>
                faker.PickRandom("Credit Card", "Cash", "Bank Transfer", "Paypal"));
            FuelTypes = GenerateList(() => faker.Vehicle.Fuel());
            Mileages = GenerateList(() => faker.PickRandom(1000));
            Statuses = GenerateList(() =>
                faker.PickRandom("Processed", "Confirmed", "Rejected", "Canceled", "Closed"));

            OfficeNames = GenerateList(() => faker.Company.CompanyName());
            Addresses = GenerateList(() => faker.Address.FullAddress());
            OccupationalPositions = GenerateList(() => faker.Name.JobTitle());
            CarModels = GenerateList(() => faker.Vehicle.Model());
        }

        private static List<T> GenerateList<T>(Func<T> generator) =>
            Enumerable.Range(1, 300).Select(_ => generator()).ToList();

        public List<Office> GenerateOffices(int count)
        {
            var offices = new List<Office>();
            for (int i = 0; i < count; i++)
            {
                offices.Add(new Office
                {
                    Name = OfficeNames.ElementAt(i),
                    Address = Addresses.ElementAt(i),
                    PhoneNumber = PhoneNumbers.ElementAt(i),
                });
            }
            Offices = offices;
            return offices;
        }

        public List<Vehicle> GenerateVehicles(int count)
        {
            var vehicles = new List<Vehicle>();
            for (int i = 0; i < count; i++)
            {
                vehicles.Add(new Vehicle
                {
                    CostPerDay = Costs.ElementAt(i),
                    FuelType = FuelTypes.ElementAt(i),
                    LicensePlate = VehicleLicensePlates.ElementAt(i),
                    Mileage = Mileages.ElementAt(i),
                    Model = CarModels.ElementAt(i),
                    OfficeName = GetRandomElementOf(Offices.Select(x => x.Name).ToList()),
                    Year = CarYears.ElementAt(i).Year
                });
            }
            Vehicles = vehicles;
            return vehicles;
        }

        public List<Worker> GenerateWorkers(int count)
        {
            var workers = new List<Worker>();
            for (int i = 0; i < count; i++)
            {
                workers.Add(new Worker
                {
                    Id = Guid.NewGuid(),
                    FirstName = FirstNames.ElementAt(i),
                    LastName = LastNames.ElementAt(i),
                    OccupationalPosition = OccupationalPositions.ElementAt(i),
                    OfficeName = GetRandomElementOf(Offices.Select(x => x.Name).ToList()),
                    PhoneNumber = PhoneNumbers.ElementAt(i)
                });
            }
            Workers = workers;
            return workers;
        }

        public List<Client> GenerateClients(int count)
        {
            var clients = new List<Client>();
            for (int i = 0; i < count; i++)
            {
                clients.Add(new Client
                {
                    FirstName = FirstNames.ElementAt(i),
                    BirthDate = BirthDates.ElementAt(i),
                    DriverLicense = DriverLicenses.ElementAt(i),
                    Email = Emails.ElementAt(i),
                    LastName = LastNames.ElementAt(i),
                    PhoneNumber = PhoneNumbers.ElementAt(i)
                });
            }
            Clients = clients;
            return clients;
        }

        public List<Rent> GenerateRents(int count)
        {
            var rents = new List<Rent>();
            for (int i = 0; i < count; i++)
            {
                var vehicle = GetRandomElementOf(Vehicles);
                rents.Add(new Rent
                {
                    Id = Guid.NewGuid(),
                    StartDate = StartDates.ElementAt(i),
                    VehicleLicensePlate = vehicle.LicensePlate,
                    EndDate = EndDates.ElementAt(i),
                    ClientEmail = GetRandomElementOf(Clients.Select(x => x.Email).ToList()),
                    WorkerId = GetRandomElementOf(Workers.Where(x => x.OfficeName == vehicle.OfficeName).Select(x => x.Id).ToList()),
                    Cost = Costs.ElementAt(i),
                    Status = Statuses.ElementAt(i),
                    Description = Descriptions.ElementAt(i)
                });
            }
            Rents = rents;
            return rents;
        }

        public List<Invoice> GenerateInvoices(int count)
        {
            var invoices = new List<Invoice>();
            for (int i = 0; i < count; i++)
            {
                invoices.Add(new Invoice
                {
                    Id = Guid.NewGuid(),
                    PayTerm = BirthDates.ElementAt(i),
                    TotalCost = TotalCosts.ElementAt(i),
                    PaymentType = PaymentTypes.ElementAt(i),
                    RentId = GetRandomElementOf(Rents.Select(x => x.Id).ToList())
                });
            }
            return invoices;
        }

        public List<InsurancePolicy> GenerateInsurancePolicies(int count)
        {
            var insurancePolicies = new List<InsurancePolicy>();
            for (int i = 0; i < count; i++)
            {
                insurancePolicies.Add(new InsurancePolicy
                {
                    Id = Guid.NewGuid(),
                    PolicyNumber = PolicyNumbers.ElementAt(i),
                    Provider = Providers.ElementAt(i),
                    Cost = Costs.ElementAt(i),
                    StartDate = StartDates.ElementAt(i),
                    EndDate = EndDates.ElementAt(i),
                    VehicleLicensePlate = GetRandomElementOf(Vehicles.Select(x => x.LicensePlate).ToList())
                });
            }

            return insurancePolicies;
        }

        public List<DamageReport> GenerateDamageReports(int count)
        {
            var damageReports = new List<DamageReport>();

            for (int i = 0; i < count; i++)
            {
                damageReports.Add(new DamageReport
                {
                    Id = Guid.NewGuid(),
                    Description = Descriptions.ElementAt(i),
                    RepairCost = Costs.ElementAt(i),
                    ReportDate = EndDates.ElementAt(i),
                    RentId = GetRandomElementOf(Rents.Select(x => x.Id).ToList())
                });
            }
            
            return damageReports;
        }

        public List<ServiceRecord> GenerateServiceRecords(int count)
        {
            var serviceRecords = new List<ServiceRecord>();
            for (int i = 0; i < count; i++)
            {
                serviceRecords.Add(new ServiceRecord
                {
                    Id = Guid.NewGuid(),
                    ServiceDate = StartDates.ElementAt(i),
                    Description = Descriptions.ElementAt(i),
                    ServiceCost = Costs.ElementAt(i),
                    VehicleLicencePlate = GetRandomElementOf(Vehicles.Select(x => x.LicensePlate).ToList())
                });
            }
            
            return serviceRecords;
        }

        public List<Review> GenerateReviews(int count)
        {
            var reviews = new List<Review>();
            for (int i = 0; i < count; i++)
            {
                var random = new Random();
                reviews.Add(new Review
                {
                    Id = Guid.NewGuid(),
                    Rating = random.Next(1, 6),
                    Comment = Descriptions.ElementAt(i),
                    ReviewDate = EndDates.ElementAt(i),
                    ClientEmail = Emails.ElementAt(i),
                    VehicleLicensePlate = GetRandomElementOf(Vehicles.Select(x => x.LicensePlate).ToList())
                });
            }
            
            return reviews;
        }

        private static T GetRandomElementOf<T>(List<T> list)
        {
            var random = new Random();
            return list.ElementAt(random.Next(list.Count));
        }
    }
}