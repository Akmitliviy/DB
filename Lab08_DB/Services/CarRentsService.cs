using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;

using Lab08DB.Data;

namespace Lab08DB
{
    public partial class CarRentsService
    {

        private readonly IDbContextFactory<CarRentsContext> contextFactory;
        private readonly NavigationManager navigationManager;

        public CarRentsService(IDbContextFactory<CarRentsContext> contextFactory, NavigationManager navigationManager)
        {
            this.contextFactory = contextFactory;
            this.navigationManager = navigationManager;
        }

        public void Reset()
        {
            using var context = contextFactory.CreateDbContext();
            contextFactory.CreateDbContext().ChangeTracker.Entries()
                .Where(e => e.Entity != null)
                .ToList()
                .ForEach(e => e.State = EntityState.Detached);
        }
        
        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }

        public async Task ExportClientsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/clients/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/clients/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportClientsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/clients/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/clients/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnClientsRead(ref IQueryable<Lab08DB.Models.CarRents.Client> items);

        public async Task<IQueryable<Lab08DB.Models.CarRents.Client>> GetClients(Query query = null)
        {
            var items = contextFactory.CreateDbContext().Clients.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnClientsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnClientGet(Lab08DB.Models.CarRents.Client item);
        partial void OnGetClientByEmail(ref IQueryable<Lab08DB.Models.CarRents.Client> items);

        public async Task<IQueryable<Lab08DB.Models.CarRents.GetClientRentHistoryResult>> GetClientRentHistoryAsync(string clientEmail)
        {
            
            var elements = contextFactory.CreateDbContext().Rents
            .Where(r => r.ClientEmail == clientEmail)
            .Select(r => new Lab08DB.Models.CarRents.GetClientRentHistoryResult
            {
                Id = r.Id,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                Status = r.Status
            })
            .ToList().AsQueryable();

            return await Task.FromResult(elements);
        
        }


        public async Task<Lab08DB.Models.CarRents.Client> GetClientByEmail(string email)
        {
            var items = contextFactory.CreateDbContext().Clients
                              .AsNoTracking()
                              .Where(i => i.Email == email);


            OnGetClientByEmail(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnClientGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnClientCreated(Lab08DB.Models.CarRents.Client item);
        partial void OnAfterClientCreated(Lab08DB.Models.CarRents.Client item);

        public async Task<Lab08DB.Models.CarRents.Client> CreateClient(Lab08DB.Models.CarRents.Client client)
        {
            OnClientCreated(client);

            var existingItem = contextFactory.CreateDbContext().Clients
                              .Where(i => i.Email == client.Email)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().Clients.Add(client);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(client).State = EntityState.Detached;
                throw;
            }

            OnAfterClientCreated(client);

            return client;
        }

        public async Task<Lab08DB.Models.CarRents.Client> CancelClientChanges(Lab08DB.Models.CarRents.Client item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnClientUpdated(Lab08DB.Models.CarRents.Client item);
        partial void OnAfterClientUpdated(Lab08DB.Models.CarRents.Client item);

        // public async Task<Lab08DB.Models.CarRents.Client> UpdateClient(string email, Lab08DB.Models.CarRents.Client client)
        // {
        //     OnClientUpdated(client);

        //     var itemToUpdate = contextFactory.CreateDbContext().Clients
        //                       .Where(i => i.Email == client.Email)
        //                       .FirstOrDefault();

        //     if (itemToUpdate == null)
        //     {
        //        throw new Exception("Item no longer available");
        //     }
                
        //     var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
        //     entryToUpdate.CurrentValues.SetValues(client);
        //     entryToUpdate.State = EntityState.Modified;

        //     contextFactory.CreateDbContext().SaveChanges();

        //     OnAfterClientUpdated(client);

        //     return client;
        // }

        public async Task<Lab08DB.Models.CarRents.Client> UpdateClient(string email, Lab08DB.Models.CarRents.Client client)
        {
            
            using var context = contextFactory.CreateDbContext();
            using var transaction = await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            //Console.WriteLine($"Transaction Isolation Level: {transaction.getDbTransaction().IsolationLevel}");
            try
            {
                OnClientUpdated(client);

                var itemToUpdate = context.Clients
                    .Where(i => i.Email == client.Email)
                    .FirstOrDefault();

                if (itemToUpdate == null)
                {
                    throw new Exception("Item no longer available");
                }

                // Оновлення значень
                context.Entry(itemToUpdate).CurrentValues.SetValues(client);
                context.Entry(itemToUpdate).Property("RowVersion").OriginalValue = client.RowVersion;

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            

                OnAfterClientUpdated(client);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine("Concurrency error: the record was modified by another user.");
                Console.WriteLine(ex.Message);

                throw new Exception("Concurrency conflict occurred. The changes were not saved.", ex);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine("An error occurred while updating the client.");
                Console.WriteLine(ex.Message);
                throw;
            }

            return client;
        }
        

        partial void OnClientDeleted(Lab08DB.Models.CarRents.Client item);
        partial void OnAfterClientDeleted(Lab08DB.Models.CarRents.Client item);

        public async Task<Lab08DB.Models.CarRents.Client> DeleteClient(string email)
        {
            var itemToDelete = contextFactory.CreateDbContext().Clients
                              .Where(i => i.Email == email)
                              .Include(i => i.Reviews)
                              .Include(i => i.Rents)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnClientDeleted(itemToDelete);


            contextFactory.CreateDbContext().Clients.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterClientDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportDamageReportsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/damagereports/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/damagereports/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportDamageReportsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/damagereports/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/damagereports/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnDamageReportsRead(ref IQueryable<Lab08DB.Models.CarRents.DamageReport> items);

        public async Task<IQueryable<Lab08DB.Models.CarRents.DamageReport>> GetDamageReports(Query query = null)
        {
            var items = contextFactory.CreateDbContext().DamageReports.AsQueryable();

            items = items.Include(i => i.Rent);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnDamageReportsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnDamageReportGet(Lab08DB.Models.CarRents.DamageReport item);
        partial void OnGetDamageReportById(ref IQueryable<Lab08DB.Models.CarRents.DamageReport> items);


        public async Task<Lab08DB.Models.CarRents.DamageReport> GetDamageReportById(Guid id)
        {
            var items = contextFactory.CreateDbContext().DamageReports
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Rent);
 
            OnGetDamageReportById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnDamageReportGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnDamageReportCreated(Lab08DB.Models.CarRents.DamageReport item);
        partial void OnAfterDamageReportCreated(Lab08DB.Models.CarRents.DamageReport item);

        public async Task<Lab08DB.Models.CarRents.DamageReport> CreateDamageReport(Lab08DB.Models.CarRents.DamageReport damagereport)
        {
            OnDamageReportCreated(damagereport);

            var existingItem = contextFactory.CreateDbContext().DamageReports
                              .Where(i => i.Id == damagereport.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().DamageReports.Add(damagereport);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(damagereport).State = EntityState.Detached;
                throw;
            }

            OnAfterDamageReportCreated(damagereport);

            return damagereport;
        }

        public async Task<Lab08DB.Models.CarRents.DamageReport> CancelDamageReportChanges(Lab08DB.Models.CarRents.DamageReport item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnDamageReportUpdated(Lab08DB.Models.CarRents.DamageReport item);
        partial void OnAfterDamageReportUpdated(Lab08DB.Models.CarRents.DamageReport item);

        public async Task<Lab08DB.Models.CarRents.DamageReport> UpdateDamageReport(Guid id, Lab08DB.Models.CarRents.DamageReport damagereport)
        {
            OnDamageReportUpdated(damagereport);

            var itemToUpdate = contextFactory.CreateDbContext().DamageReports
                              .Where(i => i.Id == damagereport.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(damagereport);
            entryToUpdate.State = EntityState.Modified;

            contextFactory.CreateDbContext().SaveChanges();

            OnAfterDamageReportUpdated(damagereport);

            return damagereport;
        }

        partial void OnDamageReportDeleted(Lab08DB.Models.CarRents.DamageReport item);
        partial void OnAfterDamageReportDeleted(Lab08DB.Models.CarRents.DamageReport item);

        public async Task<Lab08DB.Models.CarRents.DamageReport> DeleteDamageReport(Guid id)
        {
            var itemToDelete = contextFactory.CreateDbContext().DamageReports
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnDamageReportDeleted(itemToDelete);


            contextFactory.CreateDbContext().DamageReports.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterDamageReportDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportInsurancePoliciesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/insurancepolicies/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/insurancepolicies/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportInsurancePoliciesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/insurancepolicies/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/insurancepolicies/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnInsurancePoliciesRead(ref IQueryable<Lab08DB.Models.CarRents.InsurancePolicy> items);

        public async Task<IQueryable<Lab08DB.Models.CarRents.InsurancePolicy>> GetInsurancePolicies(Query query = null)
        {
            var items = contextFactory.CreateDbContext().InsurancePolicies.AsQueryable();

            items = items.Include(i => i.Vehicle);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnInsurancePoliciesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnInsurancePolicyGet(Lab08DB.Models.CarRents.InsurancePolicy item);
        partial void OnGetInsurancePolicyById(ref IQueryable<Lab08DB.Models.CarRents.InsurancePolicy> items);


        public async Task<Lab08DB.Models.CarRents.InsurancePolicy> GetInsurancePolicyById(Guid id)
        {
            var items = contextFactory.CreateDbContext().InsurancePolicies
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Vehicle);
 
            OnGetInsurancePolicyById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnInsurancePolicyGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnInsurancePolicyCreated(Lab08DB.Models.CarRents.InsurancePolicy item);
        partial void OnAfterInsurancePolicyCreated(Lab08DB.Models.CarRents.InsurancePolicy item);

        public async Task<Lab08DB.Models.CarRents.InsurancePolicy> CreateInsurancePolicy(Lab08DB.Models.CarRents.InsurancePolicy insurancepolicy)
        {
            OnInsurancePolicyCreated(insurancepolicy);

            var existingItem = contextFactory.CreateDbContext().InsurancePolicies
                              .Where(i => i.Id == insurancepolicy.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().InsurancePolicies.Add(insurancepolicy);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(insurancepolicy).State = EntityState.Detached;
                throw;
            }

            OnAfterInsurancePolicyCreated(insurancepolicy);

            return insurancepolicy;
        }

        public async Task<Lab08DB.Models.CarRents.InsurancePolicy> CancelInsurancePolicyChanges(Lab08DB.Models.CarRents.InsurancePolicy item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnInsurancePolicyUpdated(Lab08DB.Models.CarRents.InsurancePolicy item);
        partial void OnAfterInsurancePolicyUpdated(Lab08DB.Models.CarRents.InsurancePolicy item);

        public async Task<Lab08DB.Models.CarRents.InsurancePolicy> UpdateInsurancePolicy(Guid id, Lab08DB.Models.CarRents.InsurancePolicy insurancepolicy)
        {
            OnInsurancePolicyUpdated(insurancepolicy);

            var itemToUpdate = contextFactory.CreateDbContext().InsurancePolicies
                              .Where(i => i.Id == insurancepolicy.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(insurancepolicy);
            entryToUpdate.State = EntityState.Modified;

            contextFactory.CreateDbContext().SaveChanges();

            OnAfterInsurancePolicyUpdated(insurancepolicy);

            return insurancepolicy;
        }

        partial void OnInsurancePolicyDeleted(Lab08DB.Models.CarRents.InsurancePolicy item);
        partial void OnAfterInsurancePolicyDeleted(Lab08DB.Models.CarRents.InsurancePolicy item);

        public async Task<Lab08DB.Models.CarRents.InsurancePolicy> DeleteInsurancePolicy(Guid id)
        {
            var itemToDelete = contextFactory.CreateDbContext().InsurancePolicies
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnInsurancePolicyDeleted(itemToDelete);


            contextFactory.CreateDbContext().InsurancePolicies.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterInsurancePolicyDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportInvoicesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/invoices/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/invoices/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportInvoicesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/invoices/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/invoices/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnInvoicesRead(ref IQueryable<Lab08DB.Models.CarRents.Invoice> items);

        public async Task<IQueryable<Lab08DB.Models.CarRents.Invoice>> GetInvoices(Query query = null)
        {
            var items = contextFactory.CreateDbContext().Invoices.AsQueryable();

            items = items.Include(i => i.Rent);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnInvoicesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnInvoiceGet(Lab08DB.Models.CarRents.Invoice item);
        partial void OnGetInvoiceById(ref IQueryable<Lab08DB.Models.CarRents.Invoice> items);


        public async Task<Lab08DB.Models.CarRents.Invoice> GetInvoiceById(Guid id)
        {
            var items = contextFactory.CreateDbContext().Invoices
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Rent);
 
            OnGetInvoiceById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnInvoiceGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnInvoiceCreated(Lab08DB.Models.CarRents.Invoice item);
        partial void OnAfterInvoiceCreated(Lab08DB.Models.CarRents.Invoice item);

        public async Task<Lab08DB.Models.CarRents.Invoice> CreateInvoice(Lab08DB.Models.CarRents.Invoice invoice)
        {
            OnInvoiceCreated(invoice);

            var existingItem = contextFactory.CreateDbContext().Invoices
                              .Where(i => i.Id == invoice.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().Invoices.Add(invoice);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(invoice).State = EntityState.Detached;
                throw;
            }

            OnAfterInvoiceCreated(invoice);

            return invoice;
        }

        public async Task<Lab08DB.Models.CarRents.Invoice> CancelInvoiceChanges(Lab08DB.Models.CarRents.Invoice item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnInvoiceUpdated(Lab08DB.Models.CarRents.Invoice item);
        partial void OnAfterInvoiceUpdated(Lab08DB.Models.CarRents.Invoice item);

        public async Task<Lab08DB.Models.CarRents.Invoice> UpdateInvoice(Guid id, Lab08DB.Models.CarRents.Invoice invoice)
        {
            OnInvoiceUpdated(invoice);

            var itemToUpdate = contextFactory.CreateDbContext().Invoices
                              .Where(i => i.Id == invoice.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(invoice);
            entryToUpdate.State = EntityState.Modified;

            contextFactory.CreateDbContext().SaveChanges();

            OnAfterInvoiceUpdated(invoice);

            return invoice;
        }

        partial void OnInvoiceDeleted(Lab08DB.Models.CarRents.Invoice item);
        partial void OnAfterInvoiceDeleted(Lab08DB.Models.CarRents.Invoice item);

        public async Task<Lab08DB.Models.CarRents.Invoice> DeleteInvoice(Guid id)
        {
            var itemToDelete = contextFactory.CreateDbContext().Invoices
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnInvoiceDeleted(itemToDelete);


            contextFactory.CreateDbContext().Invoices.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterInvoiceDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportOfficesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/offices/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/offices/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportOfficesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/offices/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/offices/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnOfficesRead(ref IQueryable<Lab08DB.Models.CarRents.Office> items);

        public async Task<IQueryable<Lab08DB.Models.CarRents.Office>> GetOffices(Query query = null)
        {
            var items = contextFactory.CreateDbContext().Offices.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnOfficesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnOfficeGet(Lab08DB.Models.CarRents.Office item);
        partial void OnGetOfficeByName(ref IQueryable<Lab08DB.Models.CarRents.Office> items);


        public async Task<Lab08DB.Models.CarRents.Office> GetOfficeByName(string name)
        {
            var items = contextFactory.CreateDbContext().Offices
                              .AsNoTracking()
                              .Where(i => i.Name == name);

 
            OnGetOfficeByName(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnOfficeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnOfficeCreated(Lab08DB.Models.CarRents.Office item);
        partial void OnAfterOfficeCreated(Lab08DB.Models.CarRents.Office item);

        public async Task<Lab08DB.Models.CarRents.Office> CreateOffice(Lab08DB.Models.CarRents.Office office)
        {
            OnOfficeCreated(office);

            var existingItem = contextFactory.CreateDbContext().Offices
                              .Where(i => i.Name == office.Name)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().Offices.Add(office);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(office).State = EntityState.Detached;
                throw;
            }

            OnAfterOfficeCreated(office);

            return office;
        }

        public async Task<Lab08DB.Models.CarRents.Office> CancelOfficeChanges(Lab08DB.Models.CarRents.Office item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnOfficeUpdated(Lab08DB.Models.CarRents.Office item);
        partial void OnAfterOfficeUpdated(Lab08DB.Models.CarRents.Office item);

        public async Task<Lab08DB.Models.CarRents.Office> UpdateOffice(string name, Lab08DB.Models.CarRents.Office office)
        {
            OnOfficeUpdated(office);

            var itemToUpdate = contextFactory.CreateDbContext().Offices
                              .Where(i => i.Name == office.Name)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(office);
            entryToUpdate.State = EntityState.Modified;

            contextFactory.CreateDbContext().SaveChanges();

            OnAfterOfficeUpdated(office);

            return office;
        }

        partial void OnOfficeDeleted(Lab08DB.Models.CarRents.Office item);
        partial void OnAfterOfficeDeleted(Lab08DB.Models.CarRents.Office item);

        public async Task<Lab08DB.Models.CarRents.Office> DeleteOffice(string name)
        {
            var itemToDelete = contextFactory.CreateDbContext().Offices
                              .Where(i => i.Name == name)
                              .Include(i => i.Vehicles)
                              .Include(i => i.Workers)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnOfficeDeleted(itemToDelete);


            contextFactory.CreateDbContext().Offices.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterOfficeDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportRentsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/rents/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/rents/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportRentsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/rents/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/rents/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnRentsRead(ref IQueryable<Lab08DB.Models.CarRents.Rent> items);

        public async Task<IQueryable<Lab08DB.Models.CarRents.Rent>> GetRents(Query query = null)
        {
            var items = contextFactory.CreateDbContext().Rents.AsQueryable();

            items = items.Include(i => i.Client);
            items = items.Include(i => i.Vehicle);
            items = items.Include(i => i.Worker);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnRentsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnRentGet(Lab08DB.Models.CarRents.Rent item);
        partial void OnGetRentById(ref IQueryable<Lab08DB.Models.CarRents.Rent> items);


        public async Task<Lab08DB.Models.CarRents.Rent> GetRentById(Guid id)
        {
            var items = contextFactory.CreateDbContext().Rents
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Client);
            items = items.Include(i => i.Vehicle);
            items = items.Include(i => i.Worker);
 
            OnGetRentById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnRentGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnRentCreated(Lab08DB.Models.CarRents.Rent item);
        partial void OnAfterRentCreated(Lab08DB.Models.CarRents.Rent item);

        public async Task<Lab08DB.Models.CarRents.Rent> CreateRent(Lab08DB.Models.CarRents.Rent rent)
        {
            OnRentCreated(rent);

            var existingItem = contextFactory.CreateDbContext().Rents
                              .Where(i => i.Id == rent.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().Rents.Add(rent);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(rent).State = EntityState.Detached;
                throw;
            }

            OnAfterRentCreated(rent);

            return rent;
        }

        public async Task<Lab08DB.Models.CarRents.Rent> CancelRentChanges(Lab08DB.Models.CarRents.Rent item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnRentUpdated(Lab08DB.Models.CarRents.Rent item);
        partial void OnAfterRentUpdated(Lab08DB.Models.CarRents.Rent item);

        public async Task<Lab08DB.Models.CarRents.Rent> UpdateRent(Guid id, Lab08DB.Models.CarRents.Rent rent)
        {
            OnRentUpdated(rent);

            var itemToUpdate = contextFactory.CreateDbContext().Rents
                              .Where(i => i.Id == rent.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(rent);
            entryToUpdate.State = EntityState.Modified;

            contextFactory.CreateDbContext().SaveChanges();

            OnAfterRentUpdated(rent);

            return rent;
        }

        partial void OnRentDeleted(Lab08DB.Models.CarRents.Rent item);
        partial void OnAfterRentDeleted(Lab08DB.Models.CarRents.Rent item);

        public async Task<Lab08DB.Models.CarRents.Rent> DeleteRent(Guid id)
        {
            var itemToDelete = contextFactory.CreateDbContext().Rents
                              .Where(i => i.Id == id)
                              .Include(i => i.Invoices)
                              .Include(i => i.DamageReports)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnRentDeleted(itemToDelete);


            contextFactory.CreateDbContext().Rents.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterRentDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportReviewsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/reviews/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/reviews/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportReviewsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/reviews/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/reviews/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnReviewsRead(ref IQueryable<Lab08DB.Models.CarRents.Review> items);

        public async Task<IQueryable<Lab08DB.Models.CarRents.Review>> GetReviews(Query query = null)
        {
            var items = contextFactory.CreateDbContext().Reviews.AsQueryable();

            items = items.Include(i => i.Client);
            items = items.Include(i => i.Vehicle);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnReviewsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnReviewGet(Lab08DB.Models.CarRents.Review item);
        partial void OnGetReviewById(ref IQueryable<Lab08DB.Models.CarRents.Review> items);


        public async Task<Lab08DB.Models.CarRents.Review> GetReviewById(Guid id)
        {
            var items = contextFactory.CreateDbContext().Reviews
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Client);
            items = items.Include(i => i.Vehicle);
 
            OnGetReviewById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnReviewGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnReviewCreated(Lab08DB.Models.CarRents.Review item);
        partial void OnAfterReviewCreated(Lab08DB.Models.CarRents.Review item);

        public async Task<Lab08DB.Models.CarRents.Review> CreateReview(Lab08DB.Models.CarRents.Review review)
        {
            OnReviewCreated(review);

            var existingItem = contextFactory.CreateDbContext().Reviews
                              .Where(i => i.Id == review.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().Reviews.Add(review);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(review).State = EntityState.Detached;
                throw;
            }

            OnAfterReviewCreated(review);

            return review;
        }

        public async Task<Lab08DB.Models.CarRents.Review> CancelReviewChanges(Lab08DB.Models.CarRents.Review item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnReviewUpdated(Lab08DB.Models.CarRents.Review item);
        partial void OnAfterReviewUpdated(Lab08DB.Models.CarRents.Review item);

        public async Task<Lab08DB.Models.CarRents.Review> UpdateReview(Guid id, Lab08DB.Models.CarRents.Review review)
        {
            OnReviewUpdated(review);

            var itemToUpdate = contextFactory.CreateDbContext().Reviews
                              .Where(i => i.Id == review.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(review);
            entryToUpdate.State = EntityState.Modified;

            contextFactory.CreateDbContext().SaveChanges();

            OnAfterReviewUpdated(review);

            return review;
        }

        partial void OnReviewDeleted(Lab08DB.Models.CarRents.Review item);
        partial void OnAfterReviewDeleted(Lab08DB.Models.CarRents.Review item);

        public async Task<Lab08DB.Models.CarRents.Review> DeleteReview(Guid id)
        {
            var itemToDelete = contextFactory.CreateDbContext().Reviews
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnReviewDeleted(itemToDelete);


            contextFactory.CreateDbContext().Reviews.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterReviewDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportServiceRecordsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/servicerecords/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/servicerecords/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportServiceRecordsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/servicerecords/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/servicerecords/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnServiceRecordsRead(ref IQueryable<Lab08DB.Models.CarRents.ServiceRecord> items);

        public async Task<IQueryable<Lab08DB.Models.CarRents.ServiceRecord>> GetServiceRecords(Query query = null)
        {
            var items = contextFactory.CreateDbContext().ServiceRecords.AsQueryable();

            items = items.Include(i => i.Vehicle);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnServiceRecordsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnServiceRecordGet(Lab08DB.Models.CarRents.ServiceRecord item);
        partial void OnGetServiceRecordById(ref IQueryable<Lab08DB.Models.CarRents.ServiceRecord> items);


        public async Task<Lab08DB.Models.CarRents.ServiceRecord> GetServiceRecordById(Guid id)
        {
            var items = contextFactory.CreateDbContext().ServiceRecords
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Vehicle);
 
            OnGetServiceRecordById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnServiceRecordGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnServiceRecordCreated(Lab08DB.Models.CarRents.ServiceRecord item);
        partial void OnAfterServiceRecordCreated(Lab08DB.Models.CarRents.ServiceRecord item);

        public async Task<Lab08DB.Models.CarRents.ServiceRecord> CreateServiceRecord(Lab08DB.Models.CarRents.ServiceRecord servicerecord)
        {
            OnServiceRecordCreated(servicerecord);

            var existingItem = contextFactory.CreateDbContext().ServiceRecords
                              .Where(i => i.Id == servicerecord.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().ServiceRecords.Add(servicerecord);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(servicerecord).State = EntityState.Detached;
                throw;
            }

            OnAfterServiceRecordCreated(servicerecord);

            return servicerecord;
        }

        public async Task<Lab08DB.Models.CarRents.ServiceRecord> CancelServiceRecordChanges(Lab08DB.Models.CarRents.ServiceRecord item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnServiceRecordUpdated(Lab08DB.Models.CarRents.ServiceRecord item);
        partial void OnAfterServiceRecordUpdated(Lab08DB.Models.CarRents.ServiceRecord item);

        public async Task<Lab08DB.Models.CarRents.ServiceRecord> UpdateServiceRecord(Guid id, Lab08DB.Models.CarRents.ServiceRecord servicerecord)
        {
            OnServiceRecordUpdated(servicerecord);

            var itemToUpdate = contextFactory.CreateDbContext().ServiceRecords
                              .Where(i => i.Id == servicerecord.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(servicerecord);
            entryToUpdate.State = EntityState.Modified;

            contextFactory.CreateDbContext().SaveChanges();

            OnAfterServiceRecordUpdated(servicerecord);

            return servicerecord;
        }

        partial void OnServiceRecordDeleted(Lab08DB.Models.CarRents.ServiceRecord item);
        partial void OnAfterServiceRecordDeleted(Lab08DB.Models.CarRents.ServiceRecord item);

        public async Task<Lab08DB.Models.CarRents.ServiceRecord> DeleteServiceRecord(Guid id)
        {
            var itemToDelete = contextFactory.CreateDbContext().ServiceRecords
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnServiceRecordDeleted(itemToDelete);


            contextFactory.CreateDbContext().ServiceRecords.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterServiceRecordDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportVehiclesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/vehicles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/vehicles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportVehiclesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/vehicles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/vehicles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnVehiclesRead(ref IQueryable<Lab08DB.Models.CarRents.Vehicle> items);

        public async Task<IQueryable<Lab08DB.Models.CarRents.Vehicle>> GetVehicles(Query query = null)
        {
            var items = contextFactory.CreateDbContext().Vehicles.AsQueryable();

            items = items.Include(i => i.Office);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnVehiclesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnVehicleGet(Lab08DB.Models.CarRents.Vehicle item);
        partial void OnGetVehicleByLicensePlate(ref IQueryable<Lab08DB.Models.CarRents.Vehicle> items);


        public async Task<Lab08DB.Models.CarRents.Vehicle> GetVehicleByLicensePlate(string licenseplate)
        {
            var items = contextFactory.CreateDbContext().Vehicles
                              .AsNoTracking()
                              .Where(i => i.LicensePlate == licenseplate);

            items = items.Include(i => i.Office);
 
            OnGetVehicleByLicensePlate(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnVehicleGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnVehicleCreated(Lab08DB.Models.CarRents.Vehicle item);
        partial void OnAfterVehicleCreated(Lab08DB.Models.CarRents.Vehicle item);

        public async Task<Lab08DB.Models.CarRents.Vehicle> CreateVehicle(Lab08DB.Models.CarRents.Vehicle vehicle)
        {
            OnVehicleCreated(vehicle);

            var existingItem = contextFactory.CreateDbContext().Vehicles
                              .Where(i => i.LicensePlate == vehicle.LicensePlate)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().Vehicles.Add(vehicle);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(vehicle).State = EntityState.Detached;
                throw;
            }

            OnAfterVehicleCreated(vehicle);

            return vehicle;
        }

        public async Task<Lab08DB.Models.CarRents.Vehicle> CancelVehicleChanges(Lab08DB.Models.CarRents.Vehicle item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnVehicleUpdated(Lab08DB.Models.CarRents.Vehicle item);
        partial void OnAfterVehicleUpdated(Lab08DB.Models.CarRents.Vehicle item);

        public async Task<Lab08DB.Models.CarRents.Vehicle> UpdateVehicle(string licenseplate, Lab08DB.Models.CarRents.Vehicle vehicle)
        {
            OnVehicleUpdated(vehicle);

            var itemToUpdate = contextFactory.CreateDbContext().Vehicles
                              .Where(i => i.LicensePlate == vehicle.LicensePlate)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(vehicle);
            entryToUpdate.State = EntityState.Modified;

            contextFactory.CreateDbContext().SaveChanges();

            OnAfterVehicleUpdated(vehicle);

            return vehicle;
        }

        partial void OnVehicleDeleted(Lab08DB.Models.CarRents.Vehicle item);
        partial void OnAfterVehicleDeleted(Lab08DB.Models.CarRents.Vehicle item);

        public async Task<Lab08DB.Models.CarRents.Vehicle> DeleteVehicle(string licenseplate)
        {
            var itemToDelete = contextFactory.CreateDbContext().Vehicles
                              .Where(i => i.LicensePlate == licenseplate)
                              .Include(i => i.Reviews)
                              .Include(i => i.InsurancePolicies)
                              .Include(i => i.Rents)
                              .Include(i => i.ServiceRecords)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnVehicleDeleted(itemToDelete);


            contextFactory.CreateDbContext().Vehicles.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterVehicleDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportWorkersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/workers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/workers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportWorkersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/carrents/workers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/carrents/workers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnWorkersRead(ref IQueryable<Lab08DB.Models.CarRents.Worker> items);

        public async Task<IQueryable<Lab08DB.Models.CarRents.Worker>> GetWorkers(Query query = null)
        {
            var items = contextFactory.CreateDbContext().Workers.AsQueryable();

            items = items.Include(i => i.Office);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnWorkersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnWorkerGet(Lab08DB.Models.CarRents.Worker item);
        partial void OnGetWorkerById(ref IQueryable<Lab08DB.Models.CarRents.Worker> items);


        public async Task<Lab08DB.Models.CarRents.Worker> GetWorkerById(Guid id)
        {
            var items = contextFactory.CreateDbContext().Workers
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Office);
 
            OnGetWorkerById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnWorkerGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnWorkerCreated(Lab08DB.Models.CarRents.Worker item);
        partial void OnAfterWorkerCreated(Lab08DB.Models.CarRents.Worker item);

        public async Task<Lab08DB.Models.CarRents.Worker> CreateWorker(Lab08DB.Models.CarRents.Worker worker)
        {
            OnWorkerCreated(worker);

            var existingItem = contextFactory.CreateDbContext().Workers
                              .Where(i => i.Id == worker.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().Workers.Add(worker);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(worker).State = EntityState.Detached;
                throw;
            }

            OnAfterWorkerCreated(worker);

            return worker;
        }

        public async Task<Lab08DB.Models.CarRents.Worker> CancelWorkerChanges(Lab08DB.Models.CarRents.Worker item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnWorkerUpdated(Lab08DB.Models.CarRents.Worker item);
        partial void OnAfterWorkerUpdated(Lab08DB.Models.CarRents.Worker item);

        public async Task<Lab08DB.Models.CarRents.Worker> UpdateWorker(Guid id, Lab08DB.Models.CarRents.Worker worker)
        {
            OnWorkerUpdated(worker);

            var itemToUpdate = contextFactory.CreateDbContext().Workers
                              .Where(i => i.Id == worker.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(worker);
            entryToUpdate.State = EntityState.Modified;

            contextFactory.CreateDbContext().SaveChanges();

            OnAfterWorkerUpdated(worker);

            return worker;
        }

        partial void OnWorkerDeleted(Lab08DB.Models.CarRents.Worker item);
        partial void OnAfterWorkerDeleted(Lab08DB.Models.CarRents.Worker item);

        public async Task<Lab08DB.Models.CarRents.Worker> DeleteWorker(Guid id)
        {
            var itemToDelete = contextFactory.CreateDbContext().Workers
                              .Where(i => i.Id == id)
                              .Include(i => i.Rents)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnWorkerDeleted(itemToDelete);


            contextFactory.CreateDbContext().Workers.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterWorkerDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}