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
    public partial class AddServiceRecord
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

        protected override async Task OnInitializedAsync()
        {
            serviceRecord = new Lab08DB.Models.CarRents.ServiceRecord();

            vehiclesForVehicleLicencePlate = await CarRentsService.GetVehicles();
        }
        protected bool errorVisible;
        protected Lab08DB.Models.CarRents.ServiceRecord serviceRecord;

        protected IEnumerable<Lab08DB.Models.CarRents.Vehicle> vehiclesForVehicleLicencePlate;

        protected async Task FormSubmit()
        {
            try
            {
                await CarRentsService.CreateServiceRecord(serviceRecord);
                DialogService.Close(serviceRecord);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}