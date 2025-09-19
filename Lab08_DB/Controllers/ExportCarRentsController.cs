using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using Lab08DB.Data;

namespace Lab08DB.Controllers
{
    public partial class ExportCarRentsController : ExportController
    {
        private readonly CarRentsContext context;
        private readonly CarRentsService service;

        public ExportCarRentsController(CarRentsContext context, CarRentsService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/CarRents/clients/csv")]
        [HttpGet("/export/CarRents/clients/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportClientsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetClients(), Request.Query, false), fileName);
        }

        [HttpGet("/export/CarRents/clients/excel")]
        [HttpGet("/export/CarRents/clients/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportClientsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetClients(), Request.Query, false), fileName);
        }

        [HttpGet("/export/CarRents/damagereports/csv")]
        [HttpGet("/export/CarRents/damagereports/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDamageReportsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetDamageReports(), Request.Query, false), fileName);
        }

        [HttpGet("/export/CarRents/damagereports/excel")]
        [HttpGet("/export/CarRents/damagereports/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDamageReportsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetDamageReports(), Request.Query, false), fileName);
        }

        [HttpGet("/export/CarRents/insurancepolicies/csv")]
        [HttpGet("/export/CarRents/insurancepolicies/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportInsurancePoliciesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetInsurancePolicies(), Request.Query, false), fileName);
        }

        [HttpGet("/export/CarRents/insurancepolicies/excel")]
        [HttpGet("/export/CarRents/insurancepolicies/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportInsurancePoliciesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetInsurancePolicies(), Request.Query, false), fileName);
        }

        [HttpGet("/export/CarRents/invoices/csv")]
        [HttpGet("/export/CarRents/invoices/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportInvoicesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetInvoices(), Request.Query, false), fileName);
        }

        [HttpGet("/export/CarRents/invoices/excel")]
        [HttpGet("/export/CarRents/invoices/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportInvoicesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetInvoices(), Request.Query, false), fileName);
        }

        [HttpGet("/export/CarRents/offices/csv")]
        [HttpGet("/export/CarRents/offices/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOfficesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetOffices(), Request.Query, false), fileName);
        }

        [HttpGet("/export/CarRents/offices/excel")]
        [HttpGet("/export/CarRents/offices/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOfficesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetOffices(), Request.Query, false), fileName);
        }

        [HttpGet("/export/CarRents/rents/csv")]
        [HttpGet("/export/CarRents/rents/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRentsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetRents(), Request.Query, false), fileName);
        }

        [HttpGet("/export/CarRents/rents/excel")]
        [HttpGet("/export/CarRents/rents/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRentsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetRents(), Request.Query, false), fileName);
        }

        [HttpGet("/export/CarRents/reviews/csv")]
        [HttpGet("/export/CarRents/reviews/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportReviewsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetReviews(), Request.Query, false), fileName);
        }

        [HttpGet("/export/CarRents/reviews/excel")]
        [HttpGet("/export/CarRents/reviews/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportReviewsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetReviews(), Request.Query, false), fileName);
        }

        [HttpGet("/export/CarRents/servicerecords/csv")]
        [HttpGet("/export/CarRents/servicerecords/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportServiceRecordsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetServiceRecords(), Request.Query, false), fileName);
        }

        [HttpGet("/export/CarRents/servicerecords/excel")]
        [HttpGet("/export/CarRents/servicerecords/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportServiceRecordsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetServiceRecords(), Request.Query, false), fileName);
        }

        [HttpGet("/export/CarRents/vehicles/csv")]
        [HttpGet("/export/CarRents/vehicles/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportVehiclesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetVehicles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/CarRents/vehicles/excel")]
        [HttpGet("/export/CarRents/vehicles/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportVehiclesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetVehicles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/CarRents/workers/csv")]
        [HttpGet("/export/CarRents/workers/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportWorkersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetWorkers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/CarRents/workers/excel")]
        [HttpGet("/export/CarRents/workers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportWorkersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetWorkers(), Request.Query, false), fileName);
        }
    }
}
