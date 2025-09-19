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
    public partial class Vehicles
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

        protected IEnumerable<Lab08DB.Models.CarRents.Vehicle> vehicles;

        protected RadzenDataGrid<Lab08DB.Models.CarRents.Vehicle> grid0;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            vehicles = await CarRentsService.GetVehicles(new Query { Filter = $@"i => i.LicensePlate.Contains(@0) || i.Model.Contains(@0) || i.FuelType.Contains(@0) || i.OfficeName.Contains(@0)", FilterParameters = new object[] { search }, Expand = "Office" });
        }
        protected override async Task OnInitializedAsync()
        {
            vehicles = await CarRentsService.GetVehicles(new Query { Filter = $@"i => i.LicensePlate.Contains(@0) || i.Model.Contains(@0) || i.FuelType.Contains(@0) || i.OfficeName.Contains(@0)", FilterParameters = new object[] { search }, Expand = "Office" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddVehicle>("Add Vehicle", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<Lab08DB.Models.CarRents.Vehicle> args)
        {
            await DialogService.OpenAsync<EditVehicle>("Edit Vehicle", new Dictionary<string, object> { {"LicensePlate", args.Data.LicensePlate} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Lab08DB.Models.CarRents.Vehicle vehicle)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await CarRentsService.DeleteVehicle(vehicle.LicensePlate);

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
                    Detail = $"Unable to delete Vehicle"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await CarRentsService.ExportVehiclesToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "Office",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Vehicles");
            }

            if (args == null || args.Value == "xlsx")
            {
                await CarRentsService.ExportVehiclesToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "Office",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Vehicles");
            }
        }
    }
}