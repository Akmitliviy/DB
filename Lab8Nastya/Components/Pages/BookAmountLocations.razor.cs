using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Lab8Nastya.Components.Pages
{
    public partial class BookAmountLocations
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        public BookStoreService BookStoreService { get; set; }

        protected IEnumerable<Lab8Nastya.Models.BookStore.BookAmountLocation> bookAmountLocations;

        protected RadzenDataGrid<Lab8Nastya.Models.BookStore.BookAmountLocation> grid0;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            bookAmountLocations = await BookStoreService.GetBookAmountLocations(new Query { Expand = "Location,Book" });
        }
        protected override async Task OnInitializedAsync()
        {
            bookAmountLocations = await BookStoreService.GetBookAmountLocations(new Query { Expand = "Location,Book" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddBookAmountLocation>("Add BookAmountLocation", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<Lab8Nastya.Models.BookStore.BookAmountLocation> args)
        {
            await DialogService.OpenAsync<EditBookAmountLocation>("Edit BookAmountLocation", new Dictionary<string, object> { {"RecordId", args.Data.RecordId} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Lab8Nastya.Models.BookStore.BookAmountLocation bookAmountLocation)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await BookStoreService.DeleteBookAmountLocation(bookAmountLocation.RecordId);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete BookAmountLocation"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await BookStoreService.ExportBookAmountLocationsToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "Location,Book",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "BookAmountLocations");
            }

            if (args == null || args.Value == "xlsx")
            {
                await BookStoreService.ExportBookAmountLocationsToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "Location,Book",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "BookAmountLocations");
            }
        }
    }
}