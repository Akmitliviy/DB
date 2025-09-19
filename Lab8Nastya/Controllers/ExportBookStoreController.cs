using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using Lab8Nastya.Data;

namespace Lab8Nastya.Controllers
{
    public partial class ExportBookStoreController : ExportController
    {
        private readonly BookStoreContext context;
        private readonly BookStoreService service;

        public ExportBookStoreController(BookStoreContext context, BookStoreService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/BookStore/authors/csv")]
        [HttpGet("/export/BookStore/authors/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAuthorsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAuthors(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/authors/excel")]
        [HttpGet("/export/BookStore/authors/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAuthorsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAuthors(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/bookamountlocations/csv")]
        [HttpGet("/export/BookStore/bookamountlocations/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportBookAmountLocationsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetBookAmountLocations(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/bookamountlocations/excel")]
        [HttpGet("/export/BookStore/bookamountlocations/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportBookAmountLocationsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetBookAmountLocations(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/bookamountorders/csv")]
        [HttpGet("/export/BookStore/bookamountorders/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportBookAmountOrdersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetBookAmountOrders(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/bookamountorders/excel")]
        [HttpGet("/export/BookStore/bookamountorders/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportBookAmountOrdersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetBookAmountOrders(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/books/csv")]
        [HttpGet("/export/BookStore/books/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportBooksToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetBooks(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/books/excel")]
        [HttpGet("/export/BookStore/books/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportBooksToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetBooks(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/categories/csv")]
        [HttpGet("/export/BookStore/categories/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCategoriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCategories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/categories/excel")]
        [HttpGet("/export/BookStore/categories/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCategoriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCategories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/clients/csv")]
        [HttpGet("/export/BookStore/clients/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportClientsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetClients(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/clients/excel")]
        [HttpGet("/export/BookStore/clients/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportClientsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetClients(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/locations/csv")]
        [HttpGet("/export/BookStore/locations/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportLocationsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetLocations(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/locations/excel")]
        [HttpGet("/export/BookStore/locations/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportLocationsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetLocations(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/orders/csv")]
        [HttpGet("/export/BookStore/orders/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOrdersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetOrders(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/orders/excel")]
        [HttpGet("/export/BookStore/orders/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOrdersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetOrders(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/publishers/csv")]
        [HttpGet("/export/BookStore/publishers/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPublishersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetPublishers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/publishers/excel")]
        [HttpGet("/export/BookStore/publishers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPublishersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetPublishers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/workinghours/csv")]
        [HttpGet("/export/BookStore/workinghours/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportWorkingHoursToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetWorkingHours(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/workinghours/excel")]
        [HttpGet("/export/BookStore/workinghours/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportWorkingHoursToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetWorkingHours(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/getclientsbyorderstatuses/csv(, fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportGetClientsByOrderStatusesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetGetClientsByOrderStatuses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/BookStore/getclientsbyorderstatuses/excel(, fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportGetClientsByOrderStatusesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetGetClientsByOrderStatuses(), Request.Query, false), fileName);
        }
    }
}
