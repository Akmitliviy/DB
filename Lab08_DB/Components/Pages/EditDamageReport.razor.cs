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
    public partial class EditDamageReport
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
            damageReport = await CarRentsService.GetDamageReportById(Id);

            rentsForRentId = await CarRentsService.GetRents();
        }
        protected bool errorVisible;
        protected Lab08DB.Models.CarRents.DamageReport damageReport;

        protected IEnumerable<Lab08DB.Models.CarRents.Rent> rentsForRentId;

        protected async Task FormSubmit()
        {
            try
            {
                await CarRentsService.UpdateDamageReport(Id, damageReport);
                DialogService.Close(damageReport);
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