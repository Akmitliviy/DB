using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Lab08DB.Components.Pages
{
    public partial class Rents
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
        public CarRentsService CarRentsService { get; set; }

        protected IEnumerable<Lab08DB.Models.CarRents.Rent> rents;

        protected RadzenDataGrid<Lab08DB.Models.CarRents.Rent> grid0;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            rents = await CarRentsService.GetRents(new Query { Filter = $@"i => i.Status.Contains(@0) || i.Description.Contains(@0) || i.VehicleLicensePlate.Contains(@0) || i.ClientEmail.Contains(@0)", FilterParameters = new object[] { search }, Expand = "Vehicle,Client,Worker" });
        }
        protected override async Task OnInitializedAsync()
        {
            rents = await CarRentsService.GetRents(new Query { Filter = $@"i => i.Status.Contains(@0) || i.Description.Contains(@0) || i.VehicleLicensePlate.Contains(@0) || i.ClientEmail.Contains(@0)", FilterParameters = new object[] { search }, Expand = "Vehicle,Client,Worker" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddRent>("Add Rent", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<Lab08DB.Models.CarRents.Rent> args)
        {
            await DialogService.OpenAsync<EditRent>("Edit Rent", new Dictionary<string, object> { {"Id", args.Data.Id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Lab08DB.Models.CarRents.Rent rent)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await CarRentsService.DeleteRent(rent.Id);

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
                    Detail = $"Unable to delete Rent"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await CarRentsService.ExportRentsToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "Vehicle,Client,Worker",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Rents");
            }

            if (args == null || args.Value == "xlsx")
            {
                await CarRentsService.ExportRentsToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "Vehicle,Client,Worker",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Rents");
            }
        }
    }
}