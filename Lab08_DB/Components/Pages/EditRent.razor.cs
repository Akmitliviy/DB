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
    public partial class EditRent
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

        [Parameter]
        public Guid Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            rent = await CarRentsService.GetRentById(Id);

            vehiclesForVehicleLicensePlate = await CarRentsService.GetVehicles();

            clientsForClientEmail = await CarRentsService.GetClients();

            workersForWorkerId = await CarRentsService.GetWorkers();
        }
        protected bool errorVisible;
        protected Lab08DB.Models.CarRents.Rent rent;

        protected IEnumerable<Lab08DB.Models.CarRents.Vehicle> vehiclesForVehicleLicensePlate;

        protected IEnumerable<Lab08DB.Models.CarRents.Client> clientsForClientEmail;

        protected IEnumerable<Lab08DB.Models.CarRents.Worker> workersForWorkerId;

        protected async Task FormSubmit()
        {
            try
            {
                await CarRentsService.UpdateRent(Id, rent);
                DialogService.Close(rent);
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